using ApbdTutorial9.DTOs;

namespace ApbdTutorial9.Services;

public interface ISubmissionService
{
    Task<SubmissionDto> CreateSubmissionAsync(CreateSubmissionDto dto);
    Task<SubmissionDto> GradeSubmissionAsync(int submissionId, GradeSubmissionDto dto);
    Task DeleteSubmissionAsync(int submissionId);
}
