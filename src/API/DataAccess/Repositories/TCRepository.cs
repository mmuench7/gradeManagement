using API.DataAccess.Models;
using API.DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace API.DataAccess.Repositories;

public class TCRepository : ITCRepository
{
    private readonly AppDbContext _dbContext;

    public TCRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddRangeAsync(IEnumerable<TeacherCourse> links)
    {
        await _dbContext.TeacherCourses.AddRangeAsync(links);
    }

    public async Task<List<TeacherCourse>> GetByTeacherIdAsync(int teacherId)
    {
        return await _dbContext.TeacherCourses
            .Where(tc => tc.TeacherId == teacherId)
            .ToListAsync();
    }
}
