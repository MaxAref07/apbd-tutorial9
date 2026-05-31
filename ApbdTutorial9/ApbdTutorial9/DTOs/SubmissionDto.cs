namespace ApbdTutorial9.DTOs;

public class SubmissionDto
{
    public int SubmissionId { get; set; }
    public SubmissionStudentDto Student { get; set; } = null!;
    public SubmissionAssignmentDto Assignment { get; set; } = null!;
    public string RepositoryUrl { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int? Score { get; set; }
    public string? Feedback { get; set; }
}

public class SubmissionStudentDto
{
    public int StudentId { get; set; }
    public string IndexNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
}

public class SubmissionAssignmentDto
{
    public int AssignmentId { get; set; }
    public string Title { get; set; } = null!;
    public int MaxPoints { get; set; }
}
