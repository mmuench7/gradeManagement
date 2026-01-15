using Shared.DTOs.Authentication;

namespace API.Services.Abstract;

public interface IAuthService
{
    Task<AuthResponseDTO?> RegisterAsync(RegisterRequestDTO requestDTO);

    Task<AuthResponseDTO?> LoginAsync(LoginRequestDTO requestDTO);
}
