using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Dev;

public class CreateCourseRequestDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Acronym { get; set; }

    [Required]
    public int JobCategoryId { get; set; }
}
