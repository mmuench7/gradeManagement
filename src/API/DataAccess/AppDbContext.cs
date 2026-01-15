using API.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace API.DataAccess;

public class AppDbContext :DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Teacher> Teacher { get; set; }

    public DbSet<Principal> Principal { get; set; }

    public DbSet<GradeChangeRequest> GradeChangeRequest { get; set; }

    public DbSet<Grade> Grade { get; set; }

    public DbSet<TeacherJobCategory> TeacherJobCategory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Teacher>().ToTable("teacher");
        modelBuilder.Entity<Principal>().ToTable("principal");
        modelBuilder.Entity<GradeChangeRequest>().ToTable("gradechangerequest");
        modelBuilder.Entity<Grade>().ToTable("grade");

        modelBuilder.Entity<TeacherJobCategory>().ToTable("teacherjobcategory");
        modelBuilder.Entity<TeacherJobCategory>().HasKey(x => new { x.TeacherId, x.JobCategoryId });
    }
}
