using API.DataAccess.Models;
using API.DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace API.DataAccess.Repositories;

public class GradeRepository : IGradeRepository
{
    private readonly AppDbContext _dbContext;

    public GradeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Grade?> GetByIdAsync(int id)
    {
        return await _dbContext.Grade.FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task UpdateAsync(Grade grade)
    {
        _dbContext.Grade.Update(grade);
        await _dbContext.SaveChangesAsync();
    }
}
