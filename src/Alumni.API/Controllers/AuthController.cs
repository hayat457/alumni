using Alumni.Business.DTOs.Auth;
using Alumni.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Kullanıcı girişi. Başarılı olursa JWT token döner.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.LoginAsync(request);
        if (result == null)
            return Unauthorized(new { message = "E-posta veya şifre hatalı." });

        return Ok(result);
    }

    /// <summary>Yeni kullanıcı kaydı. Başarılı olursa JWT token döner.</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(request);
        if (result == null)
            return Conflict(new { message = "Bu e-posta adresi zaten kayıtlı." });

        return Created(string.Empty, result);
    }
}

