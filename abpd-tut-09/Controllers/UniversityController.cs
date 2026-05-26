using abpd_tut_09.Services;
using Microsoft.AspNetCore.Mvc;

namespace abpd_tut_09.Controllers;

[ApiController]
[Route("api")]
public class UniversityController : ControllerBase
{
    private readonly UniversityService  _universityService;

    public UniversityController(UniversityService universityService)
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
    
}