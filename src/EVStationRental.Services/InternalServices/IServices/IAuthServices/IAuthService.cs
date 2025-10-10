using EVStationRental.Common.DTOs.Authentication;
using EVStationRental.Services.Base;

namespace EVStationRental.Services.InternalServices.IServices.IAuthServices;

public interface IAuthService
{
    Task<IServiceResult> LoginAsync(LoginRequestDTO request);
    Task<IServiceResult> RefreshAsync(RefreshTokenRequestDTO request);
    Task<IServiceResult> RegisterAsync(RegisterRequestDTO request);
}


