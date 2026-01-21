using API.DataAccess.Models;
using API.DataAccess.Repositories.Abstract;
using API.Services.Abstract;
using Shared.DTOs.GradeChangeRequests;
using Shared.DTOs.Grades;

namespace API.Services;

public class GCRService : IGCRService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGradeRepository _gradeRepository;
    private readonly IGCRRepository _gcrRepository;
    private readonly ITeacherRepository _teacherRepository;

    public GCRService(
        IUnitOfWork unitOfWork,
        IGradeRepository gradeRepository,
        IGCRRepository gcrRepository,
        ITeacherRepository teacherRepository)
    {
        _unitOfWork = unitOfWork;
        _gradeRepository = gradeRepository;
        _gcrRepository = gcrRepository;
        _teacherRepository = teacherRepository;
    }

    public async Task<IGCRService.Result<GradeChangeRequestResponseDto>> CreateAsync(int teacherId, CreateGradeChangeRequestDto dto)
    {
        if (dto is null ||
            dto.GradeId <= 0 ||
            string.IsNullOrWhiteSpace(dto.Reason))
        {
            return new(false, IGCRService.Error.InvalidInput);
        }
        if (dto.RequestedGradeValue < 1 || dto.RequestedGradeValue > 6)
        {
            return new(false, IGCRService.Error.InvalidInput);
        }
        (int TeacherId, int CourseId, decimal GradeValue)? basics = await _gradeRepository.GetBasicsAsync(dto.GradeId);
        if (basics is null)
        {
            return new(false, IGCRService.Error.GradeNotFound);
        }
        (int gradeTeacherId, int courseId, decimal originalValue) = basics.Value;
        if (gradeTeacherId != teacherId)
        {
            return new(false, IGCRService.Error.Forbidden);
        }
        if (await _gcrRepository.AnyPendingForGradeAsync(dto.GradeId))
        {
            return new(false, IGCRService.Error.AlreadyPending);
        }
        int? principalId = await _gradeRepository.GetAssignedPrincipalIdByGradeIdAsync(dto.GradeId);
        if (principalId is null || principalId.Value <= 0)
        {
            return new(false, IGCRService.Error.PrincipalNotFound);
        }

        GradeChangeRequest request = new GradeChangeRequest
        {
            GradeId = dto.GradeId,
            TeacherId = teacherId,
            PrincipalId = principalId.Value,
            OriginalGradeValue = originalValue,
            RequestedGradeValue = dto.RequestedGradeValue,
            Reason = dto.Reason.Trim(),
            PrincipalComment = null,
            Status = GradeChangeRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ReviewedAt = null
        };

        try
        {
            await _gcrRepository.AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            var mapped = await MapAsync(request);
            return new(true, IGCRService.Error.None, mapped);
        }
        catch (Exception ex)
        {
            return new(
                false,
                IGCRService.Error.Failed,
                null,
                new
                {
                    message = ex.Message,
                    inner = ex.InnerException?.Message,
                    type = ex.GetType().Name
                });
        }
    }

    public async Task<IGCRService.Result<List<GradeChangeRequestResponseDto>>> GetTeacherPendingAsync(int teacherId)
    {
        try
        {
            List<GradeChangeRequest> rows = await _gcrRepository.GetTeacherPendingAsync(teacherId);
            var dtos = new List<GradeChangeRequestResponseDto>(rows.Count);
            foreach (var x in rows)
            {
                var dto = await MapAsync(x);
                dtos.Add(dto);
            }
            return new(true, IGCRService.Error.None, dtos);
        }
        catch (Exception ex)
        {
            return new(
                false,
                IGCRService.Error.Failed,
                null,
                new
                {
                    message = ex.Message,
                    inner = ex.InnerException?.Message,
                    type = ex.GetType().Name
                }
            );
        }
    }
    public async Task<IGCRService.Result<List<GradeChangeRequestResponseDto>>> GetTeacherReviewedAsync(int teacherId)
    {
        try
        {
            List<GradeChangeRequest> rows = await _gcrRepository.GetTeacherReviewedAsync(teacherId);
            var dtos = new List<GradeChangeRequestResponseDto>(rows.Count);
            foreach (var x in rows)
            {
                var dto = await MapAsync(x);
                dtos.Add(dto);
            }
            return new(true, IGCRService.Error.None, dtos);
        }
        catch (Exception ex)
        {
            return new(
                false,
                IGCRService.Error.Failed,
                null,
                new
                {
                    message = ex.Message,
                    inner = ex.InnerException?.Message,
                    type = ex.GetType().Name
                }
            );
        }
    }

    public async Task<IGCRService.Result<List<GradeChangeRequestResponseDto>>> GetPrincipalPendingAsync(int principalId)
    {
        try
        {
            List<GradeChangeRequest> rows = await _gcrRepository.GetPrincipalPendingAsync(principalId);
            var dtos = new List<GradeChangeRequestResponseDto>(rows.Count);
            foreach (var x in rows)
            {
                var dto = await MapAsync(x);
                dtos.Add(dto);
            }
            return new(true, IGCRService.Error.None, dtos);
        }
        catch (Exception ex)
        {
            return new(
                false,
                IGCRService.Error.Failed,
                null,
                new
                {
                    message = ex.Message,
                    inner = ex.InnerException?.Message,
                    type = ex.GetType().Name
                }
            );
        }
    }

    public async Task<IGCRService.Result<List<GradeChangeRequestResponseDto>>> GetPrincipalReviewedAsync(int principalId)
    {
        try
        {
            List<GradeChangeRequest> rows = await _gcrRepository.GetPrincipalReviewedAsync(principalId);
            var dtos = new List<GradeChangeRequestResponseDto>(rows.Count);
            foreach (var x in rows)
            {
                var dto = await MapAsync(x);
                dtos.Add(dto);
            }
            return new(true, IGCRService.Error.None, dtos);
        }
        catch (Exception ex)
        {
            return new(
                false,
                IGCRService.Error.Failed,
                null,
                new
                {
                    message = ex.Message,
                    inner = ex.InnerException?.Message,
                    type = ex.GetType().Name
                }
            );
        }
    }

    public async Task<IGCRService.Result<GradeChangeRequestResponseDto>> ReviewAsync(int principalId, int requestId, ReviewGradeChangeRequestDto dto)
    {
        if (requestId <= 0 || dto is null)
        {
            return new(false, IGCRService.Error.InvalidInput);
        }

        GradeChangeRequest? request = await _gcrRepository.GetByIdAsync(requestId);
        if (request is null)
        {
            return new(false, IGCRService.Error.NotFound);
        }
        if (request.PrincipalId != principalId)
        {
            return new(false, IGCRService.Error.PrincipalNotFound);
        }
        if (request.Status != GradeChangeRequestStatus.Pending)
        {
            return new(false, IGCRService.Error.NotPendingAnymore);
        }

        await using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction tx = await _unitOfWork.BeginTransactionAsync();
        try
        {
            request.Status = dto.Approve ? GradeChangeRequestStatus.Approved : GradeChangeRequestStatus.Rejected;
            request.PrincipalComment = string.IsNullOrWhiteSpace(dto.PrincipalComment) ? null : dto.PrincipalComment.Trim();
            request.ReviewedAt = DateTime.UtcNow;

            if (dto.Approve)
            {
                await _gradeRepository.UpdateGradeValueAsync(request.GradeId, request.RequestedGradeValue);
            }

            await _unitOfWork.SaveChangesAsync();
            await tx.CommitAsync();

            var mapped = await MapAsync(request);
            return new(true, IGCRService.Error.None, mapped);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return new(
                false,
                IGCRService.Error.Failed,
                null,
                new
                {
                    message = ex.Message,
                    inner = ex.InnerException?.Message,
                    type = ex.GetType().Name
                }
            );
        }
    }

    private async Task<GradeChangeRequestResponseDto> MapAsync(GradeChangeRequest x)
    {
        var dto = new GradeChangeRequestResponseDto
        {
            Id = x.Id,
            GradeId = x.GradeId,
            TeacherId = x.TeacherId,
            PrincipalId = x.PrincipalId,
            OriginalGradeValue = x.OriginalGradeValue,
            RequestedGradeValue = x.RequestedGradeValue,
            Reason = x.Reason,
            PrincipalComment = x.PrincipalComment,
            Status = x.Status.ToString(),
            CreatedAt = x.CreatedAt,
            ReviewedAt = x.ReviewedAt
        };

        // Load teacher to get name
        var teacher = await _teacherRepository.GetByIdAsync(x.TeacherId);
        if (teacher != null)
        {
            dto.TeacherName = teacher.FirstName + " " + teacher.LastName;
        }

        return dto;
    }
}
