using API.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.GradeChangeRequests;
using Shared.DTOs.Grades;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/gradechangerequest")]
[Authorize]
public class GCRController : ControllerBase
{
    private readonly IGCRService _gcrService;

    public GCRController(IGCRService gcrService)
    {
        _gcrService = gcrService;
    }

    private int UserId
    {
        get
        {
            string? raw = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out int id))
            {
                throw new InvalidOperationException("Could not extract id from token.");
            }

            return id;
        }
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Create([FromBody] CreateGradeChangeRequestDto dto)
    {
        IGCRService.Result<GradeChangeRequestResponseDto> result = await _gcrService.CreateAsync(UserId, dto);

        switch (result.Error)
        {
            case IGCRService.Error.None:
                return Created("", result.Data);

            case IGCRService.Error.InvalidInput:
                return BadRequest(new
                {
                    code = "INVALID_INPUT",
                    message = "Invalid input."
                });

            case IGCRService.Error.GradeNotFound:
                return NotFound(new
                {
                    code = "GRADE_NOT_FOUND",
                    message = "Grade not found."
                });

            case IGCRService.Error.PrincipalNotFound:
                return NotFound(new
                {
                    code = "PRINCIPAL_NOT_FOUND",
                    message = "Assigned principal not found."
                });

            case IGCRService.Error.Forbidden:
                return Forbid();

            case IGCRService.Error.AlreadyPending:
                return Conflict(new
                {
                    code = "ALREADY_PENDING",
                    message = "There is already a pending request for this grade."
                });

            default:
                if (HttpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
                {
                    return StatusCode(500, new
                    {
                        code = "FETCH_FAILED",
                        message = "Internal server error.",
                        details = result.Details
                    });
                }

                return StatusCode(500, new
                {
                    code = "FETCH_FAILED",
                    message = "Internal server error."
                });
        }
    }

    [HttpGet("teacher/pending")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetTeacherPending()
    {
        IGCRService.Result<List<GradeChangeRequestResponseDto>> result = await _gcrService.GetTeacherPendingAsync(UserId);

        switch (result.Error)
        {
            case IGCRService.Error.None:
                return Ok(result.Data);

            default:
                if (HttpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
                {
                    return StatusCode(500, new
                    {
                        code = "FETCH_FAILED",
                        message = "Internal server error.",
                        details = result.Details
                    });
                }

                return StatusCode(500, new
                {
                    code = "FETCH_FAILED",
                    message = "Internal server error."
                });
        }
    }

    [HttpGet("teacher/reviewed")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetTeacherReviewed()
    {
        IGCRService.Result<List<GradeChangeRequestResponseDto>> result = await _gcrService.GetTeacherReviewedAsync(UserId);

        switch (result.Error)
        {
            case IGCRService.Error.None:
                return Ok(result.Data);

            default:
                if (HttpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
                {
                    return StatusCode(500, new
                    {
                        code = "FETCH_FAILED",
                        message = "Internal server error.",
                        details = result.Details
                    });
                }

                return StatusCode(500, new
                {
                    code = "FETCH_FAILED",
                    message = "Internal server error."
                });
        }
    }

    [HttpGet("principal/pending")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> GetPrincipalPending()
    {
        IGCRService.Result<List<GradeChangeRequestResponseDto>> result = await _gcrService.GetPrincipalPendingAsync(UserId);

        switch (result.Error)
        {
            case IGCRService.Error.None:
                return Ok(result.Data);

            default:
                if (HttpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
                {
                    return StatusCode(500, new
                    {
                        code = "FETCH_FAILED",
                        message = "Internal server error.",
                        details = result.Details
                    });
                }

                return StatusCode(500, new
                {
                    code = "FETCH_FAILED",
                    message = "Internal server error."
                });
        }
    }

    [HttpGet("principal/reviewed")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> GetPrincipalReviewed()
    {
        IGCRService.Result<List<GradeChangeRequestResponseDto>> result = await _gcrService.GetPrincipalReviewedAsync(UserId);

        switch (result.Error)
        {
            case IGCRService.Error.None:
                return Ok(result.Data);

            default:
                if (HttpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
                {
                    return StatusCode(500, new
                    {
                        code = "FETCH_FAILED",
                        message = "Internal server error.",
                        details = result.Details
                    });
                }

                return StatusCode(500, new
                {
                    code = "FETCH_FAILED",
                    message = "Internal server error."
                });
        }
    }

    [HttpPost("{id:int}/review")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> Review([FromRoute] int id, [FromBody] ReviewGradeChangeRequestDto dto)
    {
        IGCRService.Result<GradeChangeRequestResponseDto> result = await _gcrService.ReviewAsync(UserId, id, dto);

        switch (result.Error)
        {
            case IGCRService.Error.None:
                return Ok(result.Data);

            case IGCRService.Error.InvalidInput:
                return BadRequest(new
                {
                    code = "IVALID_INPUT",
                    message = "Invalid input."
                });

            case IGCRService.Error.NotFound:
                return NotFound(new
                {
                    code = "REQUEST_NOT_FOUND",
                    message = "Grade change request not found."
                });

            case IGCRService.Error.Forbidden:
                return Forbid();

            case IGCRService.Error.NotPendingAnymore:
                return Conflict(new
                {
                    code = "NOT_PENDING",
                    message = "Request is not pending anymore."
                });

            default:
                return StatusCode(500, new
                {
                    code = "REVIEW_FAILED",
                    message = "Internal server error."
                });
        }
    }
}
