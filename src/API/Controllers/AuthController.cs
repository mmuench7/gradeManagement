using API.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Authentication;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO requestDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        AuthResponseDTO? responseDTO = await _authService.RegisterAsync(requestDTO);

        if (responseDTO == null)
        {
            return Conflict("Account already exists or registration not allowed.");
        }

        return Created(
            uri: string.Empty,
            value: responseDTO
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO requestDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        AuthResponseDTO? responseDTO = await _authService.LoginAsync(requestDTO);

        if (responseDTO == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        return Ok(responseDTO);
    }
}
