using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.GradeChangeRequest;

public class CreateGCRDTO
{
    [Required]
    public int GradeId { get; set; }

    [Required]
    public decimal RequestedGradeValue { get; set; }

    [Required]
    public string? Reason { get; set; }
}
