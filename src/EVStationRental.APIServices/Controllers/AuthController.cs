using EVStationRental.Common.DTOs.Authentication;
using EVStationRental.Services.InternalServices.IServices.IAuthServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EVStationRental.Services.Base;

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
    public async Task<ActionResult<IServiceResult>> Login([FromBody] LoginRequestDTO request)
    {
        var result = await _authService.LoginAsync(request);
        
        return result.StatusCode switch
        {
            200 => Ok(result),
            401 => Unauthorized(result),
            403 => StatusCode(403, result),
            _ => StatusCode(result.StatusCode, result)
        };
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<IServiceResult>> Refresh([FromBody] RefreshTokenRequestDTO request)
    {
        var result = await _authService.RefreshAsync(request);
        
        return result.StatusCode switch
        {
            200 => Ok(result),
            401 => Unauthorized(result),
            403 => StatusCode(403, result),
            404 => NotFound(result),
            _ => StatusCode(result.StatusCode, result)
        };
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<IServiceResult>> Register([FromBody] RegisterRequestDTO request)
    {
        var result = await _authService.RegisterAsync(request);
        
        return result.StatusCode switch
        {
            201 => StatusCode(201, result),
            200 => Ok(result),
            400 => BadRequest(result),
            409 => Conflict(result),
            _ => StatusCode(result.StatusCode, result)
        };
    }
}
