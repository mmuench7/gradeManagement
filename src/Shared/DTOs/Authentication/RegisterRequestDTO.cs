using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Authentication;

public class RegisterRequestDTO
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public int JobCategoryId { get; set; }
}
