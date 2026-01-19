using API.DataAccess.ModelsM;
using API.DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace API.DataAccess.Repositories;

public class TJCRepository : ITJCRepository
{
    private readonly AppDbContext _dbContext;

    public TJCRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddRangeAsync(IEnumerable<TeacherJobCategory> links)
    {
        await _dbContext.TeacherJobCategories.AddRangeAsync(links);
    }

    public async Task<List<TeacherJobCategory>> GetByTeacherIdAsync(int teacherId)
    {
        return await _dbContext.TeacherJobCategories
            .Where(tjc => tjc.TeacherId == teacherId)
            .ToListAsync();
    }
}
