using API.DataAccess.Models;
using API.DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace API.DataAccess.Repositories;

public class TeacherRepository : ITeacherRepository
{
    private readonly AppDbContext _dbContext;

    public TeacherRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbContext.Teacher.AnyAsync(t  => t.Email == email);
    }

    public async Task AddAsync(Teacher teacher)
    {
        await _dbContext.Teacher.AddAsync(teacher).AsTask(); 
    }

    public async Task<Teacher?> GetByEmailAsync(string email)
    {
        return await _dbContext.Teacher.SingleOrDefaultAsync(t => t.Email == email);
    }

    public async Task<Teacher?> GetByIdAsync(int id)
    {
        return await _dbContext.Teacher.FindAsync(id).AsTask();
    }
}
