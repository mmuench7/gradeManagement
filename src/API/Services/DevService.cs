using API.DataAccess.Models;
using API.DataAccess.Repositories.Abstract;
using API.Services.Abstract;
using Shared.DTOs.Dev;
using System.Security.Cryptography;
using API.Services;

namespace API.Services;

public class DevService : IDevService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPrincipalRepository _principalRepository;
    private readonly IJobCategoryRepository _jobCategoryRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ITeacherRepository _teacherRepository;

    public enum DevError
    {
        None,
        InvalidInput,
        EmailAlreadyExists,
        NotFound
    }

    public record DevResult<T>(
        bool Success,
        DevError Error,
        T? Data = default
        );

    public DevService(
        IUnitOfWork unitOfWork,
        IPrincipalRepository principalRepository,
        IJobCategoryRepository jobCategoryRepository,
        ICourseRepository courseRepository,
        ITeacherRepository teacherRepository)
    {
        _unitOfWork = unitOfWork;
        _principalRepository = principalRepository;
        _jobCategoryRepository = jobCategoryRepository;
        _courseRepository = courseRepository;
        _teacherRepository = teacherRepository;
    }

    public async Task<DevResult<object>> CreatePrincipalAsync(CreatePrincipalRequestDto requestDto)
    {
        if (requestDto is null ||
            string.IsNullOrWhiteSpace(requestDto.Email) ||
            string.IsNullOrWhiteSpace(requestDto.Password))
        {
            return new(false, DevError.InvalidInput);
        }

        if (!requestDto.Email.Trim().EndsWith("@gibz.ch", StringComparison.OrdinalIgnoreCase))
        {
            return new(false, DevError.InvalidInput);
        }

        if (await _principalRepository.ExistsByEmailAsync(requestDto.Email.Trim()) ||
            await _teacherRepository.ExistsByEmailAsync(requestDto.Email.Trim()))
        {
            return new(false, DevError.EmailAlreadyExists);
        }

        bool jobCategoryExists = await _jobCategoryRepository.ExistsByIdAsync(requestDto.JobCategoryId);
        if (!jobCategoryExists)
        {
            return new(false, DevError.NotFound);
        }

        string salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
        string hash = AuthService.BuildHash(requestDto.Password, salt);

        Principal principal = new Principal
        {
            Email = requestDto.Email.Trim(),
            FirstName = requestDto.FirstName,
            LastName = requestDto.LastName,
            PasswordSalt = salt,
            PasswordHash = hash,
            JobCategoryId = requestDto.JobCategoryId
        };

        await _principalRepository.AddAsync(principal);
        await _unitOfWork.SaveChangesAsync();

        return new(true, DevError.None, new
        {
            principal.Id,
            principal.Email,
            principal.FirstName,
            principal.LastName,
            principal.JobCategoryId
        });
    }

    public async Task<DevResult<object>> CreateJobCategoryAsync(CreateJobCategoryRequestDto requestDto)
    {
        if (requestDto is null || string.IsNullOrWhiteSpace(requestDto.Name))
        {
            return new(false, DevError.InvalidInput);
        }

        JobCategory jobCategory = new JobCategory
        {
            Name = requestDto.Name.Trim()
        };

        await _jobCategoryRepository.AddAsync(jobCategory);
        await _unitOfWork.SaveChangesAsync();

        return new(true, DevError.None, jobCategory);
    }

    public async Task<DevResult<object>> CreateCourseAsync(CreateCourseRequestDto requestDto)
    {
        if (requestDto is null ||
            string.IsNullOrWhiteSpace(requestDto.Name) ||
            requestDto.JobCategoryId <= 0)
        {
            return new(false, DevError.InvalidInput);
        }

        if (!await _jobCategoryRepository.ExistsByIdAsync(requestDto.JobCategoryId))
            return new(false, DevError.NotFound);

        Course course = new Course
        {
            Name = requestDto.Name.Trim(),
            Acronym = requestDto.Acronym?.Trim(),
            JobCategoryId = requestDto.JobCategoryId
        };

        await _courseRepository.AddAsync(course);
        await _unitOfWork.SaveChangesAsync();

        return new(true, DevError.None, course);
    }
}
