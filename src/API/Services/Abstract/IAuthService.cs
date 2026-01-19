using Shared.DTOs.Authentication;

namespace API.Services.Abstract;

public interface IAuthService
{
    Task<AuthService.AuthResult> RegisterAsync(RegisterRequestDto requestDto);

    Task<AuthService.AuthResult> LoginAsync(LoginRequestDto requestDto);
}
