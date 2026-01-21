using API.DataAccess.Models;
using API.DataAccess.ModelsM;
using Microsoft.EntityFrameworkCore;

namespace API.DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Teacher> Teacher { get; set; }

    public DbSet<TeacherJobCategory> TeacherJobCategories { get; set; }

    public DbSet<JobCategory> JobCategories { get; set; }

    public DbSet<Principal> Principal { get; set; }

    public DbSet<Course> Courses { get; set; }

    public DbSet<TeacherCourse> TeacherCourses { get; set; }

    public DbSet<GradeChangeRequest> GradeChangeRequests { get; set; }

    public DbSet<Grade> Grades { get; set; }

    public DbSet<API.DataAccess.Models.PrincipalJobCategory> PrincipalJobCategories { get; set; }

    public DbSet<API.DataAccess.Models.Student> Students { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Teacher>().ToTable("teacher");
        modelBuilder.Entity<TeacherJobCategory>().ToTable("teacher_job_category");
        modelBuilder.Entity<JobCategory>().ToTable("job_category");
        modelBuilder.Entity<Principal>().ToTable("principal");
        modelBuilder.Entity<Course>().ToTable("course");
        modelBuilder.Entity<TeacherCourse>().ToTable("teacher_course");
        modelBuilder.Entity<GradeChangeRequest>().ToTable("grade_change_request");
        modelBuilder.Entity<Grade>().ToTable("grade");

        modelBuilder.Entity<TeacherJobCategory>()
            .HasKey(x => new { x.TeacherId, x.JobCategoryId });

        modelBuilder.Entity<Course>()
            .HasOne(c => c.JobCategory)
            .WithMany(j => j.Courses)
            .HasForeignKey(c => c.JobCategoryId)
            .HasConstraintName("fk_course_jc");

        modelBuilder.Entity<TeacherCourse>()
            .HasKey(x => new { x.TeacherId, x.CourseId });

        modelBuilder.Entity<GradeChangeRequest>()
            .HasKey(gcr => gcr.Id);
        modelBuilder.Entity<GradeChangeRequest>()
            .Property(gcr => gcr.Reason)
            .IsRequired();
        modelBuilder.Entity<GradeChangeRequest>()
            .Property(gcr => gcr.PrincipalComment)
            .IsRequired(false);
        modelBuilder.Entity<GradeChangeRequest>()
            .Property(gcr => gcr.Status)
            .HasConversion<int>()
            .IsRequired();
        modelBuilder.Entity<GradeChangeRequest>()
            .Property(gcr => gcr.CreatedAt)
            .IsRequired();
        modelBuilder.Entity<GradeChangeRequest>()
            .Property(gcr => gcr.ReviewedAt)
            .IsRequired(false);
        modelBuilder.Entity<GradeChangeRequest>()
            .Property(gcr => gcr.OriginalGradeValue)
            .HasPrecision(4, 2)
            .IsRequired();
        modelBuilder.Entity<GradeChangeRequest>()
            .Property(gcr => gcr.RequestedGradeValue)
            .HasPrecision(4, 2)
            .IsRequired();
        modelBuilder.Entity<GradeChangeRequest>()
            .HasOne(gcr => gcr.Grade)
            .WithMany(g => g.GradeChangeRequests)
            .HasForeignKey(gcr => gcr.GradeId)
            .HasConstraintName("fk_gcr_grade");
        modelBuilder.Entity<GradeChangeRequest>()
            .HasOne(gcr => gcr.Teacher)
            .WithMany(t => t.GradeChangeRequests)
            .HasForeignKey(gcr => gcr.TeacherId)
            .HasConstraintName("fk_gcr_teacher");
        modelBuilder.Entity<GradeChangeRequest>()
            .HasOne(gcr => gcr.Principal)
            .WithMany(p => p.AssignedGradeChangeRequests)
            .HasForeignKey(gcr => gcr.PrincipalId)
            .HasConstraintName("fk_gcr_principal");
        modelBuilder.Entity<API.DataAccess.Models.PrincipalJobCategory>()
    .ToTable("principal_job_category");

        modelBuilder.Entity<API.DataAccess.Models.PrincipalJobCategory>()
            .HasKey(x => new { x.PrincipalId, x.JobCategoryId });
        modelBuilder.Entity<API.DataAccess.Models.Student>().ToTable("student");


    }
}
