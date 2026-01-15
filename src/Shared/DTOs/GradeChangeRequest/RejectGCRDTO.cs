using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.GradeChangeRequest;

public class RejectGCRDTO
{
    [Required]
    public string RejectionReason { get; set; }
}
