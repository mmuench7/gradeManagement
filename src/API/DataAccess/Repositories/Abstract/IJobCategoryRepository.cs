using API.DataAccess.Models;

namespace API.DataAccess.Repositories.Abstract;

public interface IJobCategoryRepository
{
    Task<bool> ExistsByIdAsync(int id);

    Task<int> CountExistingAsync(IEnumerable<int> ids);

    Task<Dictionary<int, string>> GetNamesByIdsAsync(IEnumerable<int> ids);

    Task AddAsync(JobCategory jobCategory);

    Task<List<JobCategory>> GetAllAsync();
}
