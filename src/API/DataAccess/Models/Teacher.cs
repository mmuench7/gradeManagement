using System.ComponentModel.DataAnnotations.Schema;

namespace API.DataAccess.Models;

public class Teacher
{
    public int Id { get; set; }

    public string Email { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }
    
    public string PasswordHash { get; set; }

    [NotMapped]
    public string Role { get; } = "Teacher";
}
