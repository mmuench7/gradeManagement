using API.DataAccess.Models;

namespace API.DataAccess.Repositories.Abstract;

public interface IPrincipalRepository
{
    Task<Principal?> GetByEmailAsync(string email);

    Task<bool> ExistsByEmailAsync(string email);

    Task AddAsync(Principal principal);
}
