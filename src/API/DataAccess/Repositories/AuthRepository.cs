using API.DataAccess.Models;
using API.DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace API.DataAccess.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _dbContext;

    public AuthRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Teacher?> GetTeacherByEmailAsync(string email)
    {
        return await _dbContext.Teacher.FirstOrDefaultAsync(t => t.Email == email);
    }

    public async Task<Principal?> GetPrincipalByEmailAsync(string email)
    {
        return await _dbContext.Principal.FirstOrDefaultAsync(p => p.Email == email);
    }

    public async Task<int> CreateTeacherAsync(
        string email,
        string firstName,
        string lastName,
        string passwordHash
        )
    {
        Teacher teacher = new Teacher
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = passwordHash
        };

        _dbContext.Add(teacher);
        await _dbContext.SaveChangesAsync();

        return teacher.Id;
    }

    public async Task AddTeacherJobCategoryAsync(int teacherId, int jobCategoryId)
    {
        _dbContext.TeacherJobCategory.Add(new TeacherJobCategory
        {
            TeacherId = teacherId,
            JobCategoryId = jobCategoryId
        });
        await _dbContext.SaveChangesAsync();
    }
}
