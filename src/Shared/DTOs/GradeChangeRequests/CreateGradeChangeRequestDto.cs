using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Grades;

public class CreateGradeChangeRequestDto
{
    [Required]
    public int GradeId { get; set; }

    [Required]
    [Range(1,6)]
    public decimal RequestedGradeValue { get; set; }

    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
}
