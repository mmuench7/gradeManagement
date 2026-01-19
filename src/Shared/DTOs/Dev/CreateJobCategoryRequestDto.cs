using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Dev;

public class CreateJobCategoryRequestDto
{
    [Required]
    public string Name { get; set; }
}
