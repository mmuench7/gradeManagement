namespace API.DataAccess.Repositories.Abstract;

public interface IGradeRepository
{
    Task<int?> GetAssignedPrincipalIdByGradeIdAsync(int gradeId);

    Task UpdateGradeValueAsync(int gradeId, decimal newValue);

    Task<(int TeacherId, int CourseId, decimal GradeValue)?> GetBasicsAsync(int gradeId);
}
