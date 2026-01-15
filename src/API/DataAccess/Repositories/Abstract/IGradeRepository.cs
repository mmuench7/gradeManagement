using API.DataAccess.Models;

namespace API.DataAccess.Repositories.Abstract;

public interface IGradeRepository
{
    Task<Grade?> GetByIdAsync(int id);
    Task UpdateAsync(Grade grade);
}
