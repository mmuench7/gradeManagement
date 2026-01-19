using API.DataAccess.Models;
using API.DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace API.DataAccess.Repositories;

public class JobCategoryRepository : IJobCategoryRepository
{
    private readonly AppDbContext _dbContext;

    public JobCategoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsByIdAsync(int id)
    {
        return await _dbContext.JobCategories.AnyAsync(c => c.Id == id);
    }

    public async Task<int> CountExistingAsync(IEnumerable<int> ids)
    {
        return await _dbContext.JobCategories.CountAsync(c => ids.Contains(c.Id));
    }

    public async Task<Dictionary<int, string>> GetNamesByIdsAsync(IEnumerable<int> ids)
    {
        List<int> idList = ids.Distinct().ToList();

        return await _dbContext.JobCategories
            .Where(c => idList.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name);
    }

    public async Task AddAsync(JobCategory jobCategory)
    {
        await _dbContext.JobCategories.AddAsync(jobCategory).AsTask();
    }
}
