using API.DataAccess.Models;
using API.DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace API.DataAccess.Repositories;

public class TeacherRepository : ITeacherRepository
{
    private readonly AppDbContext _dbContext;

    public TeacherRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Teacher?> GetByIdAsync(int id)
    {
        return await _dbContext.Teacher.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<int>> GetJobCategoryIdsAsync(int teacherId)
    {
        return await _dbContext.TeacherJobCategory
            .Where(x => x.TeacherId == teacherId)
            .Select(x => x.JobCategoryId)
            .ToListAsync();
    }
}
