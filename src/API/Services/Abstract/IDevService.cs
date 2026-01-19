using Shared.DTOs.Dev;

namespace API.Services.Abstract;

public interface IDevService
{
    Task<DevService.DevResult<object>> CreatePrincipalAsync(CreatePrincipalRequestDto requestDto);

    Task<DevService.DevResult<object>> CreateJobCategoryAsync(CreateJobCategoryRequestDto requestDto);

    Task<DevService.DevResult<object>> CreateCourseAsync(CreateCourseRequestDto requestDto);
}