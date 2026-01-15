using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using API.DataAccess.Models;
using API.DataAccess.Repositories.Abstract;
using API.Services.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Shared.DTOs.Authentication;

namespace API.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _config;

    private readonly PasswordHasher<PasswordHashUser> _passwordHasher = new();

    public AuthService(IAuthRepository authRepository, IConfiguration config)
    {
        _authRepository = authRepository;
        _config = config;
    }

    public async Task<AuthResponseDTO?> RegisterAsync(RegisterRequestDTO requestDTO)
    {
        string? allowedDomain = _config["AllowedAccountDomain"];
        if (!IsAllowedDomain(requestDTO.Email, allowedDomain))
        {
            return null;
        }

        if (await _authRepository.GetTeacherByEmailAsync(requestDTO.Email) != null)
        {
            return null;
        }

        if (await _authRepository.GetPrincipalByEmailAsync(requestDTO.Email) != null)
        {
            return null;
        }

        string hash = _passwordHasher.HashPassword(
            new PasswordHashUser(requestDTO.Email),
            requestDTO.Password
        );

        int teacherId = await _authRepository.CreateTeacherAsync(
            requestDTO.Email,
            requestDTO.FirstName,
            requestDTO.LastName,
            hash
        );

        await _authRepository.AddTeacherJobCategoryAsync(teacherId, requestDTO.JobCategoryId);

        (string token, DateTime expiresAtUtc) = CreateJwtToken(
            teacherId,
            requestDTO.Email,
            requestDTO.FirstName,
            requestDTO.LastName,
            "Teacher"
        );

        return new AuthResponseDTO
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            Role = "Teacher",
            UserId = teacherId,
            Email = requestDTO.Email,
            FirstName = requestDTO.FirstName,
            LastName = requestDTO.LastName
        };
    }

    public async Task<AuthResponseDTO?> LoginAsync(LoginRequestDTO requestDTO)
    {
        string? allowedDomain = _config["AllowedAccountDomain"];
        if (!IsAllowedDomain(requestDTO.Email, allowedDomain))
        {
            return null;
        }

        Teacher? teacher = await _authRepository.GetTeacherByEmailAsync(requestDTO.Email);
        if (teacher != null)
        {
            if (!VerifyPassword(teacher.Email, teacher.PasswordHash, requestDTO.Password))
            {
                return null;
            }

            (string token, DateTime expiresAtUtc) = CreateJwtToken(
                teacher.Id,
                teacher.Email,
                teacher.FirstName,
                teacher.LastName,
                teacher.Role
            );

            return new AuthResponseDTO
            {
                Token = token,
                ExpiresAtUtc = expiresAtUtc,
                Role = teacher.Role,
                UserId = teacher.Id,
                Email = teacher.Email,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName
            };
        }

        Principal? principal = await _authRepository.GetPrincipalByEmailAsync(requestDTO.Email);
        if (principal != null)
        {
            if (!VerifyPassword(principal.Email, principal.PasswordHash, requestDTO.Password))
            {
                return null;
            }

            (string token, DateTime expiresAtUtc) = CreateJwtToken(
                principal.Id,
                principal.Email,
                principal.FirstName,
                principal.LastName,
                principal.Role
            );

            return new AuthResponseDTO
            {
                Token = token,
                ExpiresAtUtc = expiresAtUtc,
                Role = principal.Role,
                UserId = principal.Id,
                Email = principal.Email,
                FirstName = principal.FirstName,
                LastName = principal.LastName
            };
        }

        return null;
    }

    private bool VerifyPassword(string email, string passwordHash, string password)
    {
        PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(
            new PasswordHashUser(email),
            passwordHash,
            password
        );

        return result != PasswordVerificationResult.Failed;
    }

    private (string token, DateTime expiresAtUtc) CreateJwtToken(int userId, string email, string firstName, string lastName, string role)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        byte[] key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

        Claim[] claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Name, $"{firstName} {lastName}"),
            new Claim(ClaimTypes.Role, role)
        };

        int expiresMinutes = int.Parse(_config["Jwt:ExpiresMinutes"]!);
        DateTime expiresAtUtc = DateTime.UtcNow.AddMinutes(expiresMinutes);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAtUtc,
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), expiresAtUtc);
    }

    private static bool IsAllowedDomain(string email, string? allowedDomain)
    {
        if (string.IsNullOrWhiteSpace(allowedDomain))
        {
            return false;
        }

        try
        {
            MailAddress address = new MailAddress(email);
            return string.Equals(address.Host, allowedDomain, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private sealed record PasswordHashUser(string Email);
}
