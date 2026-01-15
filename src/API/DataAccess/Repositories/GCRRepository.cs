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

    public async Task<int> CreateAsync(GradeChangeRequest request)
    {
        _dbContext.GradeChangeRequest.Add(request);
        await _dbContext.SaveChangesAsync();
        return request.Id;
    }

    public async Task<GradeChangeRequest?> GetByIdAsync(int id)
    {
        return await _dbContext.GradeChangeRequest.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<GradeChangeRequest>> GetByTeacherIdAsync(int teacherId)
    {
        return await _dbContext.GradeChangeRequest
            .Where(x => x.TeacherId == teacherId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<GradeChangeRequest>> GetPendingByPrincipalIdAsync(int principalId)
    {
        return await _dbContext.GradeChangeRequest
            .Where(x => x.PrincipalId == principalId && x.Status == "Pending")
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(GradeChangeRequest request)
    {
        _dbContext.GradeChangeRequest.Update(request);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<GradeChangeRequest?> GetPendingByGradeIdAsync(int gradeId)
    {
        return await _dbContext.GradeChangeRequest.FirstOrDefaultAsync(x => x.GradeId == gradeId && x.Status == "Pending");
    }

    public async Task<List<GradeChangeRequest>> GetHistoryByPrincipalIdAsync(int principalId)
    {
        return await _dbContext.GradeChangeRequest
            .Where(x => x.PrincipalId == principalId && x.Status != "Pending")
            .OrderByDescending(x => x.ReviewedAt)
            .ToListAsync();
    }
}
