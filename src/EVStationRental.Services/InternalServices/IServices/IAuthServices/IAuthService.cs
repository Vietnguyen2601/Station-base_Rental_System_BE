using EVStationRental.Common.DTOs.Authentication;

namespace EVStationRental.Services.InternalServices.IServices.IAuthServices;

public interface IAuthService
{
    Task<TokenResponseDTO> LoginAsync(LoginRequestDTO request);
    Task<TokenResponseDTO> RefreshAsync(RefreshTokenRequestDTO request);
    Task<TokenResponseDTO> RegisterAsync(RegisterRequestDTO request);
}


