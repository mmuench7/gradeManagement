using API.DataAccess.Models;
using API.DataAccess.ModelsM;

namespace API.DataAccess.Repositories.Abstract;

public interface ITJCRepository
{
    Task AddRangeAsync(IEnumerable<TeacherJobCategory> links);

    Task<List<TeacherJobCategory>> GetByTeacherIdAsync(int id);
}
