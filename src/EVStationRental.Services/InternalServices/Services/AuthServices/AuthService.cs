using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EVStationRental.Common.DTOs.Authentication;
using EVStationRental.Repositories.IRepositories;
using EVStationRental.Repositories.Models;
using EVStationRental.Services.InternalServices.IServices.IAuthServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using EVStationRental.Repositories.UnitOfWork;
using EVStationRental.Common.Helpers;
using EVStationRental.Services.Base;
using EVStationRental.Common.Enums.ServiceResultEnum;

namespace EVStationRental.Services.InternalServices.Services.AuthServices;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;
    private static readonly Dictionary<string, (Guid accountId, DateTime expiresAt)> _refreshStore = new();
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(IAuthRepository authRepository, IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        _authRepository = authRepository;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult> LoginAsync(LoginRequestDTO request)
    {
        try
        {
            var account = await _authRepository.GetAccountByUsernameAsync(request.Username);
            if (account == null)
            {
                return new ServiceResult
                {
                    StatusCode = Const.UNAUTHORIZED_ACCESS_CODE,
                    Message = "Tên đăng nhập không tồn tại"
                };
            }

            if (!account.Isactive)
            {
                return new ServiceResult
                {
                    StatusCode = Const.FORBIDDEN_ACCESS_CODE,
                    Message = "Tài khoản đã bị vô hiệu hóa"
                };
            }

            if (!PasswordHasher.Verify(request.Password, account.Password))
            {
                return new ServiceResult
                {
                    StatusCode = Const.UNAUTHORIZED_ACCESS_CODE,
                    Message = "Mật khẩu không chính xác"
                };
            }

            var tokens = await GenerateTokensAsync(account);
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_LOGIN_CODE,
                Message = Const.SUCCESS_LOGIN_MSG,
                Data = tokens
            };
        }
        catch (Exception ex)
        {
            return new ServiceResult
            {
                StatusCode = Const.ERROR_EXCEPTION,
                Message = $"Lỗi khi đăng nhập: {ex.Message}"
            };
        }
    }

    public async Task<IServiceResult> RefreshAsync(RefreshTokenRequestDTO request)
    {
        try
        {
            if (!_refreshStore.TryGetValue(request.RefreshToken, out var tuple))
            {
                return new ServiceResult
                {
                    StatusCode = Const.UNAUTHORIZED_ACCESS_CODE,
                    Message = "Refresh token không hợp lệ"
                };
            }

            if (tuple.expiresAt <= DateTime.UtcNow)
            {
                _refreshStore.Remove(request.RefreshToken);
                return new ServiceResult
                {
                    StatusCode = Const.UNAUTHORIZED_ACCESS_CODE,
                    Message = "Refresh token đã hết hạn"
                };
            }

            var account = await _authRepository.GetAccountByIdAsync(tuple.accountId);
            if (account == null)
            {
                _refreshStore.Remove(request.RefreshToken);
                return new ServiceResult
                {
                    StatusCode = Const.WARNING_NO_DATA_CODE,
                    Message = "Tài khoản không tồn tại"
                };
            }

            if (!account.Isactive)
            {
                _refreshStore.Remove(request.RefreshToken);
                return new ServiceResult
                {
                    StatusCode = Const.FORBIDDEN_ACCESS_CODE,
                    Message = "Tài khoản đã bị vô hiệu hóa"
                };
            }

            _refreshStore.Remove(request.RefreshToken);
            var tokens = await GenerateTokensAsync(account);
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_GENERATE_TOKEN_CODE,
                Message = "Làm mới token thành công",
                Data = tokens
            };
        }
        catch (Exception ex)
        {
            return new ServiceResult
            {
                StatusCode = Const.ERROR_EXCEPTION,
                Message = $"Lỗi khi làm mới token: {ex.Message}"
            };
        }
    }

    public async Task<IServiceResult> RegisterAsync(RegisterRequestDTO request)
    {
        try
        {
            // Basic uniqueness checks
            var existingByUsername = await _unitOfWork.AccountRepository.GetByUsernameAsync(request.Username);
            if (existingByUsername != null)
            {
                return new ServiceResult
                {
                    StatusCode = Const.WARNING_DATA_EXISTED_CODE,
                    Message = Const.ERROR_USERNAME_EXISTS_MSG
                };
            }

            var existingByEmail = await _unitOfWork.AccountRepository.GetByEmailAsync(request.Email);
            if (existingByEmail != null)
            {
                return new ServiceResult
                {
                    StatusCode = Const.WARNING_DATA_EXISTED_CODE,
                    Message = Const.ERROR_EMAIL_EXISTS_MSG
                };
            }

            if (!string.IsNullOrEmpty(request.ContactNumber))
            {
                var existingByPhone = await _unitOfWork.AccountRepository.GetByContactNumberAsync(request.ContactNumber);
                if (existingByPhone != null)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_DATA_EXISTED_CODE,
                        Message = Const.ERROR_PHONE_EXISTS_MSG
                    };
                }
            }

            if (!string.Equals(request.Password, request.ConfirmPassword))
            {
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_VALIDATION_CODE,
                    Message = "Mật khẩu và xác nhận mật khẩu không khớp"
                };
            }

            // Get default role (Customer) for new registrations
            var defaultRole = await _unitOfWork.RoleRepository.GetRoleByNameAsync("Customer");
            if (defaultRole == null)
            {
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = "Không tìm thấy vai trò mặc định trong hệ thống"
                };
            }

            var account = new Account
            {
                AccountId = Guid.NewGuid(),
                Username = request.Username,
                Password = PasswordHasher.Hash(request.Password),
                Email = request.Email,
                ContactNumber = request.ContactNumber,
                RoleId = defaultRole.RoleId,
                Isactive = true
            };

            // Use GenericRepository create
            _unitOfWork.AccountRepository.Create(account);

            var tokens = await GenerateTokensAsync(account);
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_REGISTER_CODE,
                Message = Const.SUCCESS_REGISTER_MSG,
                Data = tokens
            };
        }
        catch (Exception ex)
        {
            return new ServiceResult
            {
                StatusCode = Const.ERROR_EXCEPTION,
                Message = $"Lỗi khi đăng ký: {ex.Message}"
            };
        }
    }

    private async Task<TokenResponseDTO> GenerateTokensAsync(Account account)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var secret = jwtSection["Secret"] ?? string.Empty;
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var accessTokenMinutes = int.TryParse(jwtSection["AccessTokenMinutes"], out var m) ? m : 30;
        var refreshTokenDays = int.TryParse(jwtSection["RefreshTokenDays"], out var d) ? d : 7;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, account.AccountId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, account.Username)
        };

        // add role claims
        // Since the current database uses one-to-many relationship, get role from Role property
        if (account.Role != null && !string.IsNullOrWhiteSpace(account.Role.RoleName))
        {
            claims.Add(new Claim(ClaimTypes.Role, account.Role.RoleName));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(accessTokenMinutes),
            signingCredentials: creds);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        _refreshStore[refreshToken] = (account.AccountId, DateTime.UtcNow.AddDays(refreshTokenDays));

        return new TokenResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAtUtc = token.ValidTo
        };
    }
}
