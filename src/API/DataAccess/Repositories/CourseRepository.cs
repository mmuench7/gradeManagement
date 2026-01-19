using API.DataAccess.Models;
using API.DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace API.DataAccess.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _dbContext;

    public CourseRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsByIdAsync(int id)
    {
        return await _dbContext.Courses.AnyAsync(c => c.Id == id);
    }

    public async Task<int> CountExistingAsync(IEnumerable<int> ids)
    {
        return await _dbContext.Courses.CountAsync(c => ids.Contains(c.Id));
    }

    public async Task<int> CountExistingForJobCategoriesAsync(IEnumerable<int> courseIds, IEnumerable<int> jobCategoryIds)
    {
        return await _dbContext.Courses.CountAsync(c => courseIds.Contains(c.Id) && jobCategoryIds.Contains(c.JobCategoryId));
    }

    public async Task<int> CountJobCategoriesCoveredByCoursesAsync(IEnumerable<int> courseIds, IEnumerable<int> jobCategoryIds)
    {
        return await _dbContext.Courses
            .Where(c => courseIds.Contains(c.Id) && jobCategoryIds.Contains(c.JobCategoryId))
            .Select(c => c.Id)
            .Distinct()
            .CountAsync();
    }

    public async Task<List<int>> GetMissingJobCategoryIdsAsync(IEnumerable<int> courseIds, IEnumerable<int> jobCategoryIds)
    {
        List<int> jobCategoryIdList = jobCategoryIds.Distinct().ToList();
        List<int> courseIdList = courseIds.Distinct().ToList();

        List<int> coveredCategoryIds = await _dbContext.Courses
            .Where(c => courseIdList.Contains(c.Id) && jobCategoryIdList.Contains(c.JobCategoryId))
            .Select(c => c.JobCategoryId)
            .Distinct()
            .ToListAsync();

        return jobCategoryIdList
            .Except(coveredCategoryIds)
            .ToList();
    }

    public async Task AddAsync(Course course)
    {
        await _dbContext.Courses.AddAsync(course).AsTask();
    }
}
