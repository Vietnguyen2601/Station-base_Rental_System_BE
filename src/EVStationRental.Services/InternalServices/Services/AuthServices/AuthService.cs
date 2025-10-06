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

    public async Task<TokenResponseDTO> LoginAsync(LoginRequestDTO request)
    {
        var account = await _authRepository.GetAccountByUsernameAsync(request.Username);
        if (account == null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        if (!PasswordHasher.Verify(request.Password, account.Password))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        return await GenerateTokensAsync(account);
    }

    public async Task<TokenResponseDTO> RefreshAsync(RefreshTokenRequestDTO request)
    {
        if (!_refreshStore.TryGetValue(request.RefreshToken, out var tuple))
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }
        if (tuple.expiresAt <= DateTime.UtcNow)
        {
            _refreshStore.Remove(request.RefreshToken);
            throw new UnauthorizedAccessException("Expired refresh token");
        }

        var account = await _authRepository.GetAccountByIdAsync(tuple.accountId)
                      ?? throw new UnauthorizedAccessException("Account not found");

        _refreshStore.Remove(request.RefreshToken);
        return await GenerateTokensAsync(account);
    }

    public async Task<TokenResponseDTO> RegisterAsync(RegisterRequestDTO request)
    {
        // Basic uniqueness checks
        var existingByUsername = await _unitOfWork.AccountRepository.GetByUsernameAsync(request.Username);
        if (existingByUsername != null)
        {
            throw new InvalidOperationException("Username already exists");
        }
        var existingByEmail = await _unitOfWork.AccountRepository.GetByEmailAsync(request.Email);
        if (existingByEmail != null)
        {
            throw new InvalidOperationException("Email already exists");
        }

        if (!string.Equals(request.Password, request.ConfirmPassword))
        {
            throw new InvalidOperationException("Password and ConfirmPassword do not match");
        }

        var account = new Account
        {
            AccountId = Guid.NewGuid(),
            Username = request.Username,
            Password = PasswordHasher.Hash(request.Password),
            Email = request.Email,
            ContactNumber = request.ContactNumber,
            IsActive = true
        };

        // Use GenericRepository create
        _unitOfWork.AccountRepository.Create(account);

        return await GenerateTokensAsync(account);
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
        if (account.AccountRoles != null)
        {
            foreach (var ar in account.AccountRoles)
            {
                if (ar.Role != null && !string.IsNullOrWhiteSpace(ar.Role.RoleName))
                {
                    claims.Add(new Claim(ClaimTypes.Role, ar.Role.RoleName));
                }
            }
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


