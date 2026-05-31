using ApbdTutorial9.DTOs;
using ApbdTutorial9.Exceptions;
using ApbdTutorial9.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApbdTutorial9.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionService _submissionService;

    public SubmissionsController(ISubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSubmission(CreateSubmissionDto dto)
    {
        try
        {
            var submission = await _submissionService.CreateSubmissionAsync(dto);
            return CreatedAtAction(nameof(CreateSubmission), new { idSubmission = submission.SubmissionId }, submission);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
        catch (ValidationException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{idSubmission}/grade")]
    public async Task<IActionResult> GradeSubmission(int idSubmission, GradeSubmissionDto dto)
    {
        try
        {
            var submission = await _submissionService.GradeSubmissionAsync(idSubmission, dto);
            return Ok(submission);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ValidationException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{idSubmission}")]
    public async Task<IActionResult> DeleteSubmission(int idSubmission)
    {
        try
        {
            await _submissionService.DeleteSubmissionAsync(idSubmission);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ValidationException e)
        {
            return BadRequest(e.Message);
        }
    }
}
