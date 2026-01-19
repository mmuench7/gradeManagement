using Microsoft.EntityFrameworkCore.Storage;

namespace API.DataAccess.Repositories.Abstract;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
}
