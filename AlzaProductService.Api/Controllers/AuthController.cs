using AlzaProductService.Api.Auth;
using Microsoft.AspNetCore.Mvc;

namespace AlzaProductService.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly JwtTokenService _tokenService;

    public AuthController(IConfiguration config, JwtTokenService tokenService)
    {
        _config = config;
        _tokenService = tokenService;
    }

    [HttpPost("token")]
    public ActionResult<AuthResponse> Token(AuthRequest request)
    {
        var clients = _config
            .GetSection("Clients")
            .Get<List<AuthRequest>>()!;

        var isValid = clients.Any(c => c.ClientId == request.ClientId && c.ClientSecret == request.ClientSecret);

        if (!isValid)
            return Unauthorized();

        var token = _tokenService.GenerateToken(request.ClientId);

        return Ok(new AuthResponse(
            AccessToken: token,
            ExpiresIn: 3600));
    }
}