using API.DataAccess.Models;

namespace API.DataAccess.Repositories.Abstract;

public interface ITCRepository
{
    Task AddRangeAsync(IEnumerable<TeacherCourse> links);

    Task<List<TeacherCourse>> GetByTeacherIdAsync(int teacherId);
}
