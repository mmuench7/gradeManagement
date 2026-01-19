namespace API.DataAccess.Models;

public class Grade
{
    public int Id { get; set; }

    public decimal Value { get; set; }

    public DateTime ExamDate { get; set; }

    public string? Comment { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public int TeacherId { get; set; }

    public ICollection<GradeChangeRequest> GradeChangeRequests { get; set; } = new List<GradeChangeRequest>();
}
