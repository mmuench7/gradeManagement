using API.DataAccess.Models;

namespace API.DataAccess.Repositories.Abstract;

public interface IGCRRepository
{
    Task<int> CreateAsync(GradeChangeRequest request);

    Task<GradeChangeRequest?> GetByIdAsync(int id);

    Task<List<GradeChangeRequest>> GetByTeacherIdAsync(int teacherId);

    Task<List<GradeChangeRequest>> GetPendingByPrincipalIdAsync(int principalId);

    Task UpdateAsync(GradeChangeRequest request);

    Task<GradeChangeRequest?> GetPendingByGradeIdAsync(int gradeId);

    Task<List<GradeChangeRequest>> GetHistoryByPrincipalIdAsync(int principalId);
}
