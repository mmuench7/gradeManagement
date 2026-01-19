using API.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Authentication;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto requestDto)
    {
        Services.AuthService.AuthResult result = await _authService.RegisterAsync(requestDto);

        if (result.Success)
        {
            return StatusCode(201, result.Data);
        }

        switch (result.Error)
        {
            case Services.AuthService.AuthError.EmailAlreadyExists:
                return Conflict(new
                {
                    code = "ACCOUNT_ALREADY_EXISTS",
                    message = "Account already exists."
                });

            case Services.AuthService.AuthError.NoDataProvided:
                return BadRequest(new
                {
                    code = "NO_DATA",
                    message = "Email and password are required."
                });

            case Services.AuthService.AuthError.InvalidCredentials:
                return Unauthorized(new
                {
                    code = "INVALID_CREDENTIALS",
                    message = "Invalid credentials."
                });

            case Services.AuthService.AuthError.InvalidJobCategories:
                return NotFound(new
                {
                    code = "INVALID_JOB_CATEGORIES",
                    message = "Job categories not found."
                });

            case Services.AuthService.AuthError.InvalidEmailDomain:
                return Unauthorized(new
                {
                    code = "INVALID_EMAIL_DOMAIN",
                    message = "Invalid email domain."
                });

            case Services.AuthService.AuthError.InvalidCourses:
                return NotFound(new
                {
                    code = "INVALID_COURSES",
                    message = "Courses not found."
                });

            case Services.AuthService.AuthError.CoursesDoNotMatchJobCategories:
                return BadRequest(new
                {
                    code = "COURSES_JOBCATEGORY_MISMATCH",
                    message = "One or more selected courses do not belong to the selected job categories."
                });

            case Services.AuthService.AuthError.MissingCoursesForJobCategories:
                return BadRequest(new
                {
                    code = "MISSING_COURSES_FOR_JOB_CATEGORIES",
                    message = "You must select at least one course for every selected job category.",
                    details = result.Details
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

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto requestDto)
    {
        Services.AuthService.AuthResult result = await _authService.LoginAsync(requestDto);

        if (result.Success)
        {
            return Ok(result.Data);
        }

        switch (result.Error)
        {
            case Services.AuthService.AuthError.NoDataProvided:
                return BadRequest(new
                {
                    code = "NO_DATA",
                    message = "Email and password are required."
                });

            case Services.AuthService.AuthError.InvalidCredentials:
                return Unauthorized(new
                {
                    code = "INVALID_CREDENTIALS",
                    message = "Email or password is wrong."
                });

            default:
                if (HttpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        code = "UNEXPECTED",
                        message = "Unexpected internal server error.",
                        details = result.Details
                    });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    code = "UNEXPECTED",
                    message = "Unexpected internal server error."
                });
        }
    }
}
