using EVStationRental.Common.DTOs.Authentication;
using EVStationRental.Services.InternalServices.IServices.IAuthServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EVStationRental.APIServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDTO>> Login([FromBody] LoginRequestDTO request)
    {
        var tokens = await _authService.LoginAsync(request);
        return Ok(tokens);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<TokenResponseDTO>> Refresh([FromBody] RefreshTokenRequestDTO request)
    {
        var tokens = await _authService.RefreshAsync(request);
        return Ok(tokens);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<TokenResponseDTO>> Register([FromBody] RegisterRequestDTO request)
    {
        var tokens = await _authService.RegisterAsync(request);
        return Ok(tokens);
    }
}


