using abpd_tut_09.Data;
using abpd_tut_09.DTOs;
using abpd_tut_09.Exceptions;
using abpd_tut_09.Models;
using Microsoft.EntityFrameworkCore;

namespace abpd_tut_09.Services;

public class UniversityService
{
    private readonly UniversityTasksDbContext _db;

    public UniversityService(UniversityTasksDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<CourseDto>> GetCourses(bool activeOnly)
    {
        return await _db.Courses
            .AsNoTracking()
            .Where(c => c.IsActive || !activeOnly)
            .Select(c => new CourseDto
            {
                CourseId = c.CourseId,
                AssignmentCount = c.Assignments.Count,
                Code = c.Code,
                Credits = c.Credits,
                Name = c.Name
            })
            .ToListAsync();
    }
    
    public async Task<IEnumerable<AssignmentDto>> GetAssignmentsForCourse(int id,  bool publishedOnly = false)
    {
        if (! await _db.Courses.AnyAsync(c => c.CourseId == id))
        {
            throw new NotFoundException("Course not found");
        }
        return await _db.Assignments
            .AsNoTracking()
            .Where(a => a.CourseId == id && (a.IsPublished || !publishedOnly))
            .Select(a => new AssignmentDto
            {
                IsPublished = a.IsPublished,
                AssignmentId = a.AssignmentId,
                DueDate = a.DueDate,
                MaxPoints = a.MaxPoints,
                SubmissionCount = a.Submissions.Count,
                Title = a.Title
            })
            .ToListAsync();
    }
    
    public async Task<StudentDashboardDto> GetStudentDashboard(int id)
    {
        if (! await _db.Students.AnyAsync(s => s.StudentId == id))
        {
            throw new NotFoundException("Student not found");
        }

        return await _db.Students
            .AsNoTracking()
            .Where(s => s.StudentId == id)
            .Select(s => new StudentDashboardDto
            {
                StudentId = s.StudentId,
                IsActive = s.IsActive,
                FullName = s.FullName,
                IndexNumber = s.IndexNumber,
                Submissions = s.Submissions.Select(submission => new DashboardSubmissionDto
                {
                    SubmissionId = submission.SubmissionId,
                    AssignmentId = submission.AssignmentId,
                    Feedback = submission.Feedback,
                    RepositoryUrl = submission.RepositoryUrl,
                    Score = submission.Score,
                    Status = submission.Status,
                    SubmittedAt = submission.SubmittedAt

                }).ToList(),
                Enrollments = s.Enrollments.Select(e => new DashboardEnrollmentDto
                {
                    EnrollmentId = e.EnrollmentId,
                    CourseId = e.CourseId,
                    Status = e.Status,
                    EnrolledAt = e.EnrolledAt
                }).ToList()
            })
            .FirstAsync();

    }
    
    
}