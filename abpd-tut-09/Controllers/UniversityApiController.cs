using abpd_tut_09.DTOs;
using abpd_tut_09.Services;
using Microsoft.AspNetCore.Mvc;

namespace abpd_tut_09.Controllers;

[ApiController]
[Route("api")]
public class UniversityApiController : ControllerBase
{
    private readonly IUniversityService  _universityService;

    public UniversityApiController(IUniversityService universityService)
    {
        _universityService = universityService;
    }

    [HttpGet("courses")]
    public async Task<IActionResult> GetCourses(bool activeOnly = false)
    {
        return Ok(await _universityService.GetCourses(activeOnly));
    }

    [HttpGet("courses/{id:int}/assignments")]
    public async Task<IActionResult> GetAssignmentsForCourse(int id,  bool publishedOnly = false)
    {
        return Ok(await _universityService.GetAssignmentsForCourse(id, publishedOnly));
    }

    [HttpGet("students/{id:int}/dashboard")]
    public async Task<IActionResult> GetStudentDashboard(int id)
    {
        return Ok(await _universityService.GetStudentDashboard(id));
    }

    [HttpPost("submissions")]
    public async Task<IActionResult> CreateSubmission([FromBody] CreateSubmissionDto createSubmissionDto)
    {
        if (string.IsNullOrWhiteSpace(createSubmissionDto.RepositoryUrl))
            return BadRequest("RepositoryUrl cannot be blank.");

        if (!createSubmissionDto.RepositoryUrl.StartsWith("https://"))
            return BadRequest("RepositoryUrl must start with https://");
        
        var res = await _universityService.CreateSubmission(createSubmissionDto);
        return CreatedAtAction(nameof(GetStudentDashboard), new { id = res.StudentDto.StudentId }, res);
    }

    [HttpPut("submissions/{id:int}/grade")]
    public async Task<IActionResult> GradeSubmission(int id, [FromBody] GradeSubmissionDto gradeSubmissionDto)
    {
        return Ok(await _universityService.GradeSubmission(id, gradeSubmissionDto));
    }

    [HttpDelete("submissions/{id:int}")]
    public async Task<IActionResult> DeleteSubmission(int id)
    {
        await  _universityService.DeleteSubmission(id);
        return NoContent();
    }
    
}