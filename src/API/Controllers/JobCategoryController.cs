using API.DataAccess.Repositories.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobCategoryController : ControllerBase
{
 private readonly IJobCategoryRepository _jobCategoryRepository;

 public JobCategoryController(IJobCategoryRepository jobCategoryRepository)
 {
 _jobCategoryRepository = jobCategoryRepository;
 }

 [HttpGet]
 public async Task<IActionResult> GetAll()
 {
 var list = await _jobCategoryRepository.GetAllAsync();
 var dto = list.Select(c => new { c.Id, c.Name }).ToList();
 return Ok(dto);
 }
}