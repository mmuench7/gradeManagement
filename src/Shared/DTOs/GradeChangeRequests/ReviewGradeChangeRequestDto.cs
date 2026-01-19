using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.GradeChangeRequests;

public class ReviewGradeChangeRequestDto
{
    [Required]
    public bool Approve { get; set; }

    [Required]
    [MaxLength(500)]
    public string? PrincipalComment { get; set; }
}
