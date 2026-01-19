namespace API.DataAccess.Models;

public class Course
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Acronym { get; set; }

    public int JobCategoryId { get; set; }

    public JobCategory JobCategory { get; set; }
}
