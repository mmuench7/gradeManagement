using API.DataAccess.Models;
using API.DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace API.DataAccess.Repositories;

public class PrincipalRepository : IPrincipalRepository
{
    private readonly AppDbContext _dbContext;

    public PrincipalRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbContext.Principal.AnyAsync(p => p.Email == email);
    }

    public async Task<Principal?> GetByEmailAsync(string email)
    {
        return await _dbContext.Principal.SingleOrDefaultAsync(p => p.Email == email);
    }

    public async Task AddAsync(Principal principal)
    {
        await _dbContext.Principal.AddAsync(principal).AsTask();
    }
}
