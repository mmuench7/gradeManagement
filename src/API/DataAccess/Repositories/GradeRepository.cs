using API.DataAccess.Models;
using API.DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace API.DataAccess.Repositories;

public class GradeRepository : IGradeRepository
{
    private readonly AppDbContext _dbContext;

    public GradeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(int TeacherId, int CourseId, decimal GradeValue)?> GetBasicsAsync(int gradeId)
    {
        var row = await _dbContext.Grades
            .Where(g => g.Id == gradeId)
            .Select(g => new { g.TeacherId, g.CourseId, GradeValue = g.Value })
            .FirstOrDefaultAsync();

        return row is null ? null : (row.TeacherId, row.CourseId, row.GradeValue);
    }
    public async Task<int?> GetAssignedPrincipalIdByGradeIdAsync(int gradeId)
    {
        return await _dbContext.Grades
            .Where(g => g.Id == gradeId)
            .Join(_dbContext.Courses,
                  g => g.CourseId,
                  c => c.Id,
                  (g, c) => c.JobCategoryId)
            .Join(_dbContext.PrincipalJobCategories,
                  jobCategoryId => jobCategoryId,
                  pjc => pjc.JobCategoryId,
                  (jobCategoryId, pjc) => pjc.PrincipalId)
            .FirstOrDefaultAsync();
    }


    public async Task UpdateGradeValueAsync(int gradeId, decimal newValue)
    {
        Grade? grade = await _dbContext.Grades.FirstOrDefaultAsync(g => g.Id == gradeId);
        if (grade is null)
        {
            return;
        }

        grade.Value = newValue;
    }
}
