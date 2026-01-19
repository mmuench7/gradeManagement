using API.DataAccess.Models;

namespace API.DataAccess.Repositories.Abstract;

public interface IGCRRepository
{
    Task AddAsync(GradeChangeRequest request);

    Task<GradeChangeRequest?> GetByIdAsync(int id);

    Task<List<GradeChangeRequest>> GetTeacherPendingAsync(int teacherId);

    Task<List<GradeChangeRequest>> GetTeacherReviewedAsync(int teacherId);

    Task<List<GradeChangeRequest>> GetPrincipalPendingAsync(int principalId);

    Task<List<GradeChangeRequest>> GetPrincipalReviewedAsync(int principalId);

    Task<bool> AnyPendingForGradeAsync(int gradeId);
}
