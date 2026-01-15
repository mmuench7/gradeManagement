using API.DataAccess.Models;

namespace API.DataAccess.Repositories.Abstract;

public interface IAuthRepository
{
    Task<Teacher?> GetTeacherByEmailAsync(string email);

    Task<Principal?> GetPrincipalByEmailAsync(string email);

    Task<int> CreateTeacherAsync(
        string email,
        string firstName,
        string lastName,
        string passwordHash
        );

    Task AddTeacherJobCategoryAsync(int teacherId, int jobCategoryId);
}
