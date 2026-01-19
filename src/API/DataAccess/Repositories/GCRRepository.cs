using API.DataAccess.Models;
using API.DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace API.DataAccess.Repositories;

public class GCRRepository : IGCRRepository
{
    private readonly AppDbContext _dbContext;

    public GCRRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(GradeChangeRequest request)
    {
        return _dbContext.GradeChangeRequests.AddAsync(request).AsTask();
    }

    public Task<GradeChangeRequest?> GetByIdAsync(int id)
    {
        return _dbContext.GradeChangeRequests.FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<bool> AnyPendingForGradeAsync(int gradeId)
    {
        return _dbContext.GradeChangeRequests.AnyAsync(x =>
            x.GradeId == gradeId &&
            x.Status == GradeChangeRequestStatus.Pending);
    }

    public Task<List<GradeChangeRequest>> GetTeacherPendingAsync(int teacherId)
    {
        return _dbContext.GradeChangeRequests
            .Where(x => x.TeacherId == teacherId && x.Status == GradeChangeRequestStatus.Pending)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public Task<List<GradeChangeRequest>> GetTeacherReviewedAsync(int teacherId)
    {
        return _dbContext.GradeChangeRequests
            .Where(x => x.TeacherId == teacherId &&
                x.Status == GradeChangeRequestStatus.Approved ||
                x.Status == GradeChangeRequestStatus.Rejected)
            .OrderByDescending(x => x.ReviewedAt)
            .ToListAsync();
    }

    public Task<List<GradeChangeRequest>> GetPrincipalPendingAsync(int principalId)
    {
        return _dbContext.GradeChangeRequests
            .Where(x => x.PrincipalId == principalId && x.Status == GradeChangeRequestStatus.Pending)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public Task<List<GradeChangeRequest>> GetPrincipalReviewedAsync(int principalId)
    {
        return _dbContext.GradeChangeRequests
            .Where(x => x.PrincipalId== principalId &&
                x.Status == GradeChangeRequestStatus.Approved ||
                x.Status == GradeChangeRequestStatus.Rejected)
            .OrderByDescending(x => x.ReviewedAt)
            .ToListAsync();
    }
}
