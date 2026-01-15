using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Authentication;

public class AuthResponseDTO
{
    [Required]
    public string Token { get; set; }

    [Required]
    public DateTime ExpiresAtUtc { get; set; }

    [Required]
    public string Role {  get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }
}
