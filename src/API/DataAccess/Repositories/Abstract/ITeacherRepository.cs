using API.DataAccess.Models;

namespace API.DataAccess.Repositories.Abstract;

public interface ITeacherRepository
{
    Task<Teacher?> GetByIdAsync(int id);

    Task<List<int>> GetJobCategoryIdsAsync(int teacherId);
}
