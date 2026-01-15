using API.DataAccess.Models;

namespace API.DataAccess.Repositories.Abstract;

public interface IPrincipalRepository
{
    Task<Principal?> GetByIdAsync(int id);

    Task<Principal?> GetByJobCategoryIdAsync(int jobCategoryId);
}
