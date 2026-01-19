namespace API.DataAccess.Models;

public enum GradeChangeRequestStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}

public class GradeChangeRequest
{
    public int Id { get; set; }

    public int GradeId { get; set; }

    public int TeacherId { get; set; }

    public int PrincipalId { get; set; }

    public decimal OriginalGradeValue { get; set; }

    public decimal RequestedGradeValue { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string? PrincipalComment { get; set; }

    public GradeChangeRequestStatus Status { get; set; } = GradeChangeRequestStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }

    public Teacher Teacher { get; set; }

    public Principal Principal { get; set; }

    public Grade Grade { get; set; }
}
