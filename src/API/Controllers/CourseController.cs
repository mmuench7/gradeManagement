using API.DataAccess.Repositories.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
 private readonly ICourseRepository _courseRepository;

 public CourseController(ICourseRepository courseRepository)
 {
 _courseRepository = courseRepository;
 }

 [HttpGet]
 public async Task<IActionResult> GetAll([FromQuery] int[]? jobCategoryIds)
 {
 var list = await _courseRepository.GetByJobCategoryIdsAsync(jobCategoryIds ?? Array.Empty<int>());
 var dto = list.Select(c => new { c.Id, c.Name, c.Acronym, c.JobCategoryId }).ToList();
 return Ok(dto);
 }
}