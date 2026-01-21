using API.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/grades")]
[Authorize(Roles = "Teacher")]
public class GradesController : ControllerBase
{
    private readonly AppDbContext _db;

    public GradesController(AppDbContext db)
    {
        _db = db;
    }

    private int UserId
    {
        get
        {
            string? raw = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out int id))
                throw new InvalidOperationException("Could not extract id from token.");
            return id;
        }
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        int teacherId = UserId;

        var rows = await _db.Grades
            .Where(g => g.TeacherId == teacherId)
            .Join(_db.Students,
                g => g.StudentId,
                s => s.Id,
                (g, s) => new { g, s })
            .Join(_db.Courses,
                x => x.g.CourseId,
                c => c.Id,
                (x, c) => new
                {
                    GradeId = x.g.Id,
                    StudentName = x.s.FirstName + " " + x.s.LastName,
                    CourseName = c.Name,
                    CourseAcronym = c.Acronym,
                    CurrentValue = x.g.Value,
                    ExamDate = x.g.ExamDate
                })
            .OrderByDescending(x => x.ExamDate)
            .ToListAsync();

        return Ok(rows);
    }
}
