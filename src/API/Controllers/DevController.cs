using API.Services;
using API.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Dev;

namespace API.Controllers;

[ApiController]
[Route("api/dev")]
[Authorize(Policy = "DevAdminOnly")]
public class DevController : ControllerBase
{
    private readonly IDevService _devService;

    public DevController(IDevService devService)
    {
        _devService = devService;
    }

    [HttpPost("principals")]
    public async Task<IActionResult> CreatePrincipal([FromBody] CreatePrincipalRequestDto requestDto)
    {
        DevService.DevResult<object> result = await _devService.CreatePrincipalAsync(requestDto);

        switch(result.Error)
        {
            case DevService.DevError.None:
                return Created("", result.Data);

            case DevService.DevError.InvalidInput:
                return BadRequest(new
                {
                    code = "INVALID_INPUT",
                    message = "Invalid input."
                });

            case DevService.DevError.EmailAlreadyExists:
                return Conflict(new
                {
                    code = "EMAIL_ALREADY_EXISTS",
                    message = "Email already exists."
                });

            case DevService.DevError.NotFound:
                return NotFound(new
                {
                    code = "JOB_CATEGORY_NOT_FOUND",
                    message = "Referenced job category not found."
                });

            default:
                return StatusCode(500, new
                {
                    code = "DEV_CREATE_PRINCIPAL_FAILED",
                    message = "Internal server error."
                });
        }
    }

    [HttpPost("jobcategories")]
    public async Task<IActionResult> CreateJobCategory([FromBody] CreateJobCategoryRequestDto requestDto)
    {
        DevService.DevResult<object> result = await _devService.CreateJobCategoryAsync(requestDto);

        switch (result.Error)
        {
            case DevService.DevError.None:
                return Created("", result.Data);

            case DevService.DevError.InvalidInput:
                return BadRequest(new
                {
                    code = "INVALID_INPUT",
                    message = "Invalid input."
                });

            default:
                return StatusCode(500, new
                {
                    code = "DEV_CREATE_JOBCATEGORY_FAILED",
                    message = "Internal server error."
                });
        }
    }

    [HttpPost("courses")]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequestDto requestDto)
    {
        DevService.DevResult<object> result = await _devService.CreateCourseAsync(requestDto);

        switch (result.Error)
        {
            case DevService.DevError.None:
                return Created("", result.Data);

            case DevService.DevError.InvalidInput:
                return BadRequest(new
                {
                    code = "INVALID_INPUT",
                    message = "Invalid input."
                });

            case DevService.DevError.NotFound:
                return NotFound(new
                {
                    code = "JOB_CATEGORY_NOT_FOUND",
                    message = "Referenced job category not found."
                });

            default:
                return StatusCode(500, new
                {
                    code = "DEV_CREATE_COURSE_FAILED",
                    message = "Internal server error."
                });
        }
    }
}
