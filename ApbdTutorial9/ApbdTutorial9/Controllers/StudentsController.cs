using ApbdTutorial9.Data;
using ApbdTutorial9.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApbdTutorial9.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly UniversityTasksDbContext _context;

    public StudentsController(UniversityTasksDbContext context)
    {
        _context = context;
    }

    [HttpGet("{idStudent}/dashboard")]
    public async Task<IActionResult> GetDashboard(int idStudent)
    {
        var dashboard = await _context.Students
            .AsNoTracking()
            .Where(s => s.StudentId == idStudent)
            .Select(s => new StudentDashboardDto
            {
                StudentId = s.StudentId,
                IndexNumber = s.IndexNumber,
                FullName = s.FirstName + " " + s.LastName,
                IsActive = s.IsActive,
                Enrollments = s.Enrollments.Select(e => new DashboardEnrollmentDto
                {
                    CourseId = e.CourseId,
                    CourseCode = e.Course.Code,
                    CourseName = e.Course.Name,
                    Status = e.Status
                }).ToList(),
                Submissions = s.Submissions.Select(sub => new DashboardSubmissionDto
                {
                    SubmissionId = sub.SubmissionId,
                    AssignmentId = sub.AssignmentId,
                    AssignmentTitle = sub.Assignment.Title,
                    Status = sub.Status,
                    Score = sub.Score
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (dashboard is null)
            return NotFound($"Student {idStudent} not found.");

        return Ok(dashboard);
    }
}
