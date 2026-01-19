using API.DataAccess.Models;

namespace API.DataAccess.Repositories.Abstract;

public interface ITeacherRepository
{
    Task<bool> ExistsByEmailAsync(string email);

    Task AddAsync(Teacher teacher);

    Task<Teacher?> GetByEmailAsync(string email);
}
