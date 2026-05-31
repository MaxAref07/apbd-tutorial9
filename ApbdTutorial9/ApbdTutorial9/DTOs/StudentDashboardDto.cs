namespace ApbdTutorial9.DTOs;

public class StudentDashboardDto
{
    public int StudentId { get; set; }
    public string IndexNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public bool IsActive { get; set; }
    public List<DashboardEnrollmentDto> Enrollments { get; set; } = new();
    public List<DashboardSubmissionDto> Submissions { get; set; } = new();
}

public class DashboardEnrollmentDto
{
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public string Status { get; set; } = null!;
}

public class DashboardSubmissionDto
{
    public int SubmissionId { get; set; }
    public int AssignmentId { get; set; }
    public string AssignmentTitle { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int? Score { get; set; }
}
