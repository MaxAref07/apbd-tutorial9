using ApbdTutorial9.Data;
using ApbdTutorial9.DTOs;
using ApbdTutorial9.Exceptions;
using ApbdTutorial9.Models;
using Microsoft.EntityFrameworkCore;

namespace ApbdTutorial9.Services;

public class SubmissionService : ISubmissionService
{
    private readonly UniversityTasksDbContext _context;

    public SubmissionService(UniversityTasksDbContext context)
    {
        _context = context;
    }

    public async Task<SubmissionDto> CreateSubmissionAsync(CreateSubmissionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RepositoryUrl) ||
            !dto.RepositoryUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationException("RepositoryUrl must not be blank and must start with https://.");
        }

        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == dto.StudentId);

        if (student is null)
            throw new NotFoundException($"Student {dto.StudentId} not found.");

        if (!student.IsActive)
            throw new ValidationException("Student is not active.");

        var assignment = await _context.Assignments
            .FirstOrDefaultAsync(a => a.AssignmentId == dto.AssignmentId);

        if (assignment is null)
            throw new NotFoundException($"Assignment {dto.AssignmentId} not found.");

        if (!assignment.IsPublished)
            throw new ValidationException("Assignment is not published.");

        var enrolled = await _context.Enrollments
            .AnyAsync(e => e.StudentId == dto.StudentId
                           && e.CourseId == assignment.CourseId
                           && (e.Status == "Active" || e.Status == "Completed"));

        if (!enrolled)
            throw new ValidationException("Student is not enrolled in the course that owns the assignment.");

        var alreadySubmitted = await _context.Submissions
            .AnyAsync(s => s.AssignmentId == dto.AssignmentId && s.StudentId == dto.StudentId);

        if (alreadySubmitted)
            throw new ConflictException("Student has already submitted this assignment.");

        var now = DateTime.Now;

        var submission = new Submission
        {
            AssignmentId = dto.AssignmentId,
            StudentId = dto.StudentId,
            RepositoryUrl = dto.RepositoryUrl,
            SubmittedAt = now,
            Status = assignment.IsOverdue(now) ? "Late" : "Submitted"
        };

        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();

        return await GetSubmissionDtoAsync(submission.SubmissionId);
    }

    public async Task<SubmissionDto> GradeSubmissionAsync(int submissionId, GradeSubmissionDto dto)
    {
        var submission = await _context.Submissions
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

        if (submission is null)
            throw new NotFoundException($"Submission {submissionId} not found.");

        if (dto.Score < 0)
            throw new ValidationException("Score cannot be lower than 0.");

        if (dto.Score > submission.Assignment.MaxPoints)
            throw new ValidationException($"Score cannot be higher than {submission.Assignment.MaxPoints}.");

        submission.Score = dto.Score;
        submission.Feedback = dto.Feedback;
        submission.Status = "Graded";

        await _context.SaveChangesAsync();

        return await GetSubmissionDtoAsync(submission.SubmissionId);
    }

    public async Task DeleteSubmissionAsync(int submissionId)
    {
        var submission = await _context.Submissions
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

        if (submission is null)
            throw new NotFoundException($"Submission {submissionId} not found.");

        if (submission.Status == "Graded")
            throw new ValidationException("A graded submission cannot be deleted.");

        _context.Submissions.Remove(submission);
        await _context.SaveChangesAsync();
    }

    private async Task<SubmissionDto> GetSubmissionDtoAsync(int submissionId)
    {
        return await _context.Submissions
            .AsNoTracking()
            .Where(s => s.SubmissionId == submissionId)
            .Select(s => new SubmissionDto
            {
                SubmissionId = s.SubmissionId,
                RepositoryUrl = s.RepositoryUrl,
                Status = s.Status,
                Score = s.Score,
                Feedback = s.Feedback,
                Student = new SubmissionStudentDto
                {
                    StudentId = s.Student.StudentId,
                    IndexNumber = s.Student.IndexNumber,
                    FullName = s.Student.FirstName + " " + s.Student.LastName
                },
                Assignment = new SubmissionAssignmentDto
                {
                    AssignmentId = s.Assignment.AssignmentId,
                    Title = s.Assignment.Title,
                    MaxPoints = s.Assignment.MaxPoints
                }
            })
            .FirstAsync();
    }
}
