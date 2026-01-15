namespace API.DataAccess.Models;

public class GradeChangeRequest
{
    public int Id { get; set; }

    public int GradeId { get; set; }

    public int TeacherId { get; set; }

    public int PrincipalId  { get; set; }

    public decimal OriginalGradeValue { get; set; }

    public decimal RequestedGradeValue { get; set; }

    public string? Reason { get; set; }

    public string Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? RejectionReason { get; set; }
}
