using API.DataAccess.Models;
using API.DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace API.DataAccess.Repositories;

public class PrincipalRepository : IPrincipalRepository
{
    private readonly AppDbContext _dbContext;

    public PrincipalRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Principal?> GetByIdAsync(int id)
    {
        return await _dbContext.Principal.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Principal?> GetByJobCategoryIdAsync(int jobCategoryId)
    {
        return await _dbContext.Principal.FirstOrDefaultAsync(p => p.JobCategoryId == jobCategoryId);
    }
}
