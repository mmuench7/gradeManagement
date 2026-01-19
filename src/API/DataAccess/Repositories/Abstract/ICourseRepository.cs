using API.DataAccess.Models;

namespace API.DataAccess.Repositories.Abstract;

public interface ICourseRepository
{
    Task<bool> ExistsByIdAsync(int id);

    Task<int> CountExistingAsync(IEnumerable<int> ids);

    Task<int> CountExistingForJobCategoriesAsync(IEnumerable<int> courseIds, IEnumerable<int> jobCategoryIds);

    Task<int> CountJobCategoriesCoveredByCoursesAsync(IEnumerable<int> courseIds, IEnumerable<int> jobCategoryIds);

    Task<List<int>> GetMissingJobCategoryIdsAsync(IEnumerable<int> courseIds, IEnumerable<int> jobCategoryIds);

    Task AddAsync(Course course);
}
