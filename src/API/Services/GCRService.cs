using API.DataAccess.Models;
using API.DataAccess.Repositories.Abstract;
using API.Services.Abstract;
using Shared.DTOs.GradeChangeRequest;

namespace API.Services;

public class GCRService : IGCRService
{
    private readonly IGCRRepository _requestRepo;
    private readonly IGradeRepository _gradeRepo;
    private readonly ITeacherRepository _teacherRepo;
    private readonly IPrincipalRepository _principalRepo;

    public GCRService(
        IGCRRepository requestRepo,
        IGradeRepository gradeRepo,
        ITeacherRepository teacherRepo,
        IPrincipalRepository principalRepo)
    {
        _requestRepo = requestRepo;
        _gradeRepo = gradeRepo;
        _teacherRepo = teacherRepo;
        _principalRepo = principalRepo;
    }

    public async Task<int?> CreateAsync(int teacherId, CreateGCRDTO dto)
    {
        // Teacher must exist
        Teacher? teacher = await _teacherRepo.GetByIdAsync(teacherId);
        if (teacher == null)
            return null;

        // Grade must exist
        Grade? grade = await _gradeRepo.GetByIdAsync(dto.GradeId);
        if (grade == null)
            return null;

        // Prevent duplicate pending request for the same grade (optional improvement you added)
        GradeChangeRequest? existingPending = await _requestRepo.GetPendingByGradeIdAsync(grade.Id);
        if (existingPending != null)
            return null;

        // Teacher may only request changes for their own grades
        if (grade.TeacherId == null || grade.TeacherId != teacherId)
            return null;

        // Validate requested grade range
        if (dto.RequestedGradeValue < 1.0m || dto.RequestedGradeValue > 6.0m)
            return null;

        // Teacher can have 1 or 2 job categories via join table
        List<int> categoryIds = await _teacherRepo.GetJobCategoryIdsAsync(teacherId);
        if (categoryIds.Count == 0)
            return null;

        // TEMP RULE:
        // If teacher has 2 categories, pick one deterministically (lowest id).
        // Better routing is: grade -> course -> course.JobCategoryId -> principal (if you have that column).
        int chosenCategoryId = categoryIds.OrderBy(x => x).First();

        // Find principal for that category
        Principal? principal = await _principalRepo.GetByJobCategoryIdAsync(chosenCategoryId);
        if (principal == null)
            return null;

        GradeChangeRequest request = new GradeChangeRequest
        {
            GradeId = grade.Id,
            TeacherId = teacher.Id,
            PrincipalId = principal.Id,
            OriginalGradeValue = grade.GradeValue,
            RequestedGradeValue = dto.RequestedGradeValue,
            Reason = dto.Reason,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            ReviewedAt = null,
            RejectionReason = null
        };

        int id = await _requestRepo.CreateAsync(request);
        return id;
    }

    public async Task<GCRDTO?> GetByIdAsync(int requestId)
    {
        GradeChangeRequest? request = await _requestRepo.GetByIdAsync(requestId);
        if (request == null)
            return null;

        return MapToDto(request);
    }

    public async Task<List<GCRDTO>> GetMineAsync(int teacherId)
    {
        List<GradeChangeRequest> items = await _requestRepo.GetByTeacherIdAsync(teacherId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<GCRDTO>> GetPendingForPrincipalAsync(int principalId)
    {
        List<GradeChangeRequest> items = await _requestRepo.GetPendingByPrincipalIdAsync(principalId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<GCRDTO>> GetHistoryForPrincipalAsync(int principalId)
    {
        List<GradeChangeRequest> items = await _requestRepo.GetHistoryByPrincipalIdAsync(principalId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<bool> ApproveAsync(int principalId, int requestId)
    {
        GradeChangeRequest? request = await _requestRepo.GetByIdAsync(requestId);
        if (request == null)
            return false;

        if (request.PrincipalId != principalId)
            return false;

        if (request.Status != "Pending")
            return false;

        Grade? grade = await _gradeRepo.GetByIdAsync(request.GradeId);
        if (grade == null)
            return false;

        // Optimistic check: grade must still match what it was when the request was created
        if (grade.GradeValue != request.OriginalGradeValue)
            return false;

        // Apply grade change
        grade.GradeValue = request.RequestedGradeValue;
        await _gradeRepo.UpdateAsync(grade);

        // Update request status
        request.Status = "Approved";
        request.ReviewedAt = DateTime.UtcNow;
        request.RejectionReason = null;

        await _requestRepo.UpdateAsync(request);
        return true;
    }

    public async Task<bool> RejectAsync(int principalId, int requestId, string rejectionReason)
    {
        GradeChangeRequest? request = await _requestRepo.GetByIdAsync(requestId);
        if (request == null)
            return false;

        if (request.PrincipalId != principalId)
            return false;

        if (request.Status != "Pending")
            return false;

        request.Status = "Rejected";
        request.RejectionReason = rejectionReason;
        request.ReviewedAt = DateTime.UtcNow;

        await _requestRepo.UpdateAsync(request);
        return true;
    }

    private static GCRDTO MapToDto(GradeChangeRequest x)
    {
        return new GCRDTO
        {
            Id = x.Id,
            GradeId = x.GradeId,
            TeacherId = x.TeacherId,
            PrincipalId = x.PrincipalId,
            OriginalGradeValue = x.OriginalGradeValue,
            RequestedGradeValue = x.RequestedGradeValue,
            Reason = x.Reason,
            Status = x.Status,
            CreatedAt = x.CreatedAt,
            ReviewedAt = x.ReviewedAt,
            RejectionReason = x.RejectionReason
        };
    }
}
