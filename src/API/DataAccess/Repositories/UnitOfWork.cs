using API.DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.DataAccess.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public UnitOfWork(AppDbContext appDbContext)
    {
        _dbContext = appDbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        return _dbContext.Database.BeginTransactionAsync(ct);
    }
}
