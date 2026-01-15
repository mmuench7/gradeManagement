using Shared.DTOs.GradeChangeRequest;

namespace API.Services.Abstract;

public interface IGCRService
{
    Task<int?> CreateAsync(int teacherId, CreateGCRDTO dto);

    Task<GCRDTO?> GetByIdAsync(int requestId);

    Task<List<GCRDTO>> GetMineAsync(int teacherId);

    Task<List<GCRDTO>> GetPendingForPrincipalAsync(int principalId);

    Task<List<GCRDTO>> GetHistoryForPrincipalAsync(int principalId);

    Task<bool> ApproveAsync(int principalId, int requestId);

    Task<bool> RejectAsync(int principalId, int requestId, string rejectionReason);
}
