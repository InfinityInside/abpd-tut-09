using abpd_tut_09.Data;
using abpd_tut_09.DTOs;
using abpd_tut_09.Exceptions;
using abpd_tut_09.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace abpd_tut_09.Services;

public interface IUniversityService
{

    public Task<IEnumerable<CourseDto>> GetCourses(bool activeOnly);

    public Task<IEnumerable<AssignmentDto>> GetAssignmentsForCourse(int id, bool publishedOnly = false);

    public Task<StudentDashboardDto> GetStudentDashboard(int id);

    public Task<SubmissionDto> CreateSubmission(CreateSubmissionDto createSubmissionDto);

    public Task<SubmissionDto> GradeSubmission(int id, GradeSubmissionDto gradeSubmissionDto);

    public Task DeleteSubmission(int id);


}