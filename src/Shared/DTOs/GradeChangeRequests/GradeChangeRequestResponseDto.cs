namespace Shared.DTOs.Grades;

public class GradeChangeRequestResponseDto
{
    public int Id { get; set; }

    public int GradeId { get; set; }

    public int TeacherId { get; set; }

    public int PrincipalId { get; set; }

    public decimal OriginalGradeValue { get; set; }

    public decimal RequestedGradeValue { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string? PrincipalComment { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }
}
