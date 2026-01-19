namespace API.DataAccess.Models;

public class Teacher
{
    public int Id { get; set; }

    public string Email { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string PasswordHash { get; set; }

    public string PasswordSalt { get; set; }

    public ICollection<GradeChangeRequest> GradeChangeRequests { get; set; } = new List<GradeChangeRequest>();
}
