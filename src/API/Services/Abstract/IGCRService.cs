using Shared.DTOs.GradeChangeRequests;
using Shared.DTOs.Grades;

namespace API.Services.Abstract;

public interface IGCRService
{
    Task<Result<GradeChangeRequestResponseDto>> CreateAsync(int teacherId, CreateGradeChangeRequestDto dto);

    Task<Result<List<GradeChangeRequestResponseDto>>> GetTeacherPendingAsync(int teacherId);

    Task<Result<List<GradeChangeRequestResponseDto>>> GetTeacherReviewedAsync(int teacherId);

    Task<Result<List<GradeChangeRequestResponseDto>>> GetPrincipalPendingAsync(int principalId);

    Task<Result<List<GradeChangeRequestResponseDto>>> GetPrincipalReviewedAsync(int principalId);

    Task<Result<GradeChangeRequestResponseDto>> ReviewAsync(int principalId, int requestId, ReviewGradeChangeRequestDto dto);

    public record Result<T>(
        bool Success,
        Error Error,
        T? Data = default,
        Object? Details = null);

    public enum Error
    {
        None,
        InvalidInput,
        GradeNotFound,
        Forbidden,
        AlreadyPending,
        PrincipalNotFound,
        Failed,
        NotFound,
        NotPendingAnymore
    }
}
