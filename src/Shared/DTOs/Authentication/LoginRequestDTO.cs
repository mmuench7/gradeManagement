using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Authentication;

public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
