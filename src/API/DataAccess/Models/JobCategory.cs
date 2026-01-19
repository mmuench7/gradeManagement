namespace API.DataAccess.Models;

public class JobCategory
{
    public int Id { get; set; }

    public string Name { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
