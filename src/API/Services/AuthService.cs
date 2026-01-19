using API.DataAccess;
using API.DataAccess.Models;
using API.DataAccess.ModelsM;
using API.DataAccess.Repositories.Abstract;
using API.Services.Abstract;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using Shared.DTOs.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace API.Services;

public class AuthService : IAuthService
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly IConfiguration _configuration;
    private readonly IJobCategoryRepository _jobCategoryRepository;
    private readonly ITJCRepository _tjcRepository;
    private readonly IPrincipalRepository _principalRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ITCRepository _tcRepository;
    private readonly IUnitOfWork _unitOfWork;

    public record AuthResult(
        bool Success,
        AuthError Error,
        AuthResponseDto? Data = null,
        object? Details = null
        );

    public enum AuthError
    {
        None,
        InvalidCredentials,
        EmailAlreadyExists,
        InvalidJobCategories,
        NoDataProvided,
        InvalidEmailDomain,
        InvalidCourses,
        CoursesDoNotMatchJobCategories,
        MissingCoursesForJobCategories,
        Failed
    }

    public enum UserType
    {
        Teacher,
        Principal
    }

    public AuthService(
        ITeacherRepository teacherRepository,
        IConfiguration configuration,
        IJobCategoryRepository jobCategoryRepository,
        ITJCRepository tjcRepository,
        IPrincipalRepository principalRepository,
        ICourseRepository courseRepository,
        ITCRepository tcRepository,
        IUnitOfWork unitOfWork)
    {
        _teacherRepository = teacherRepository;
        _configuration = configuration;
        _jobCategoryRepository = jobCategoryRepository;
        _tjcRepository = tjcRepository;
        _principalRepository = principalRepository;
        _courseRepository = courseRepository;
        _tcRepository = tcRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequestDto requestDto)
    {
        if (await _teacherRepository.ExistsByEmailAsync(requestDto.Email) ||
            await _principalRepository.ExistsByEmailAsync(requestDto.Email))
        {
            return new(false, AuthError.EmailAlreadyExists);
        }
        if (string.IsNullOrWhiteSpace(requestDto.Password) ||
                 string.IsNullOrWhiteSpace(requestDto.Email))
        {
            return new(false, AuthError.NoDataProvided);
        }
        if (!requestDto.Email.EndsWith("@gibz.ch", StringComparison.OrdinalIgnoreCase))
        {
            return new(false, AuthError.InvalidEmailDomain);
        }

        List<int> jobCategoryIds = requestDto.JobCategoryIds?
            .Distinct()
            .ToList() ?? new List<int>();
        if (jobCategoryIds.Count == 0)
        {
            return new(false, AuthError.InvalidJobCategories);
        }

        int existingJobCategoriesCount = await _jobCategoryRepository.CountExistingAsync(jobCategoryIds);
        if (existingJobCategoriesCount != jobCategoryIds.Count)
        {
            return new(false, AuthError.InvalidJobCategories);
        }

        List<int> courseIds = requestDto.CourseIds?
            .Distinct()
            .ToList() ?? new List<int>();
        if (courseIds.Count == 0)
        {
            return new(false, AuthError.InvalidCourses);
        }

        int existingCoursesCount = await _courseRepository.CountExistingAsync(courseIds);
        if (existingCoursesCount != courseIds.Count)
        {
            return new(false, AuthError.InvalidCourses);
        }

        int validCoursesForCategoriesCount = await _courseRepository.CountExistingForJobCategoriesAsync(courseIds, jobCategoryIds);
        if (validCoursesForCategoriesCount != courseIds.Count)
        {
            return new(false, AuthError.CoursesDoNotMatchJobCategories);
        }

        int coveredCategoryCount = await _courseRepository.CountJobCategoriesCoveredByCoursesAsync(courseIds, jobCategoryIds);
        if (coveredCategoryCount != jobCategoryIds.Count)
        {
            List<int> missingCategoryIds = await _courseRepository.GetMissingJobCategoryIdsAsync(courseIds, jobCategoryIds);
            if (missingCategoryIds.Count > 0)
            {
                Dictionary<int, string> names = await _jobCategoryRepository.GetNamesByIdsAsync(missingCategoryIds);
                
                return new(false, AuthError.MissingCoursesForJobCategories, null, new
                {
                    missingJobCategories = names.Select(kv => new { id = kv.Key, name = kv.Value }).ToList()
                });
            }
        }

        await using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction tx = await _unitOfWork.BeginTransactionAsync();
        try
        {
            string salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
            string hash = BuildHash(requestDto.Password, salt);

            Teacher teacher = new Teacher
            {
                Email = requestDto.Email,
                FirstName = requestDto.FirstName,
                LastName = requestDto.LastName,
                PasswordHash = hash,
                PasswordSalt = salt
            };
            await _teacherRepository.AddAsync(teacher);
            await _unitOfWork.SaveChangesAsync();

            IEnumerable<TeacherJobCategory> teacherJobCategoryLinks = jobCategoryIds.Select(id => new TeacherJobCategory
            {
                TeacherId = teacher.Id,
                JobCategoryId = id
            });
            await _tjcRepository.AddRangeAsync(teacherJobCategoryLinks);

            IEnumerable<TeacherCourse> teacherCourseLinks = courseIds.Select(courseId => new TeacherCourse
            {
                TeacherId = teacher.Id,
                CourseId = courseId
            });
            await _tcRepository.AddRangeAsync(teacherCourseLinks);

            await _unitOfWork.SaveChangesAsync();
            await tx.CommitAsync();

            return await AuthenticateAsync(requestDto.Email, requestDto.Password);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return new(
                false,
                AuthError.Failed,
                null,
                new
                {
                    message = ex.Message,
                    inner = ex.InnerException?.Message,
                    type = ex.GetType().Name
                });
        }
    }

    public async Task<AuthResult> LoginAsync(LoginRequestDto requestDto)
    {
        return await AuthenticateAsync(requestDto.Email, requestDto.Password);
    }

    private async Task<AuthResult> AuthenticateAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return new(false, AuthError.NoDataProvided);
        }

        email = email.Trim();

        Teacher teacher = await _teacherRepository.GetByEmailAsync(email);
        if (teacher != null)
        {
            if (!VerifyPassword(password, teacher.PasswordSalt, teacher.PasswordHash))
            {
                return new(false, AuthError.InvalidCredentials);
            }

            string token = GenerateJwtToken(
                userId: teacher.Id,
                email: teacher.Email,
                fullName: $"{teacher.FirstName} {teacher.LastName}",
                userType: UserType.Teacher
            );

            return new(true, AuthError.None, new AuthResponseDto
            {
                JwtToken = token,
                UserType = UserType.Teacher.ToString()
            });
        }

        Principal principal = await _principalRepository.GetByEmailAsync(email);
        if (principal != null)
        {
            if (!VerifyPassword(password, principal.PasswordSalt, principal.PasswordHash))
            {
                return new(false, AuthError.InvalidCredentials);
            }

            string token = GenerateJwtToken(
                userId: principal.Id,
                email: principal.Email,
                fullName: $"{principal.FirstName} {principal.LastName}",
                userType: UserType.Principal
            );

            return new(true, AuthError.None, new AuthResponseDto
            {
                JwtToken = token,
                UserType = UserType.Principal.ToString()
            });
        }

        return new(false, AuthError.InvalidCredentials);
    }

    private static bool VerifyPassword(string password, string salt, string expectedHash)
    {
        string hash = BuildHash(password, salt);

        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(hash),
            Convert.FromBase64String(expectedHash));
    }

    private string GenerateJwtToken(int userId, string email, string fullName, UserType userType)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        byte[] key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

        Claim[] claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Name, fullName),
            new Claim("role", userType.ToString()),
            new("userType", userType.ToString())
        };

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(30),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
                )
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static string BuildHash(string password, string salt)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: Convert.FromBase64String(salt),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8
            ));
    }
}
