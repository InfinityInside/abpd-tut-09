using abpd_tut_09.Data;
using abpd_tut_09.DTOs;
using abpd_tut_09.Exceptions;
using abpd_tut_09.Models;
using Microsoft.EntityFrameworkCore;

namespace abpd_tut_09.Services;

public class UniversityService : IUniversityService
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
    
    public async Task<SubmissionDto> CreateSubmission(CreateSubmissionDto createSubmissionDto)
    {
        var student = await _db.Students
            .AsNoTracking()
            .Include(s => s.Enrollments)
            .Include(s => s.Submissions)
            .FirstOrDefaultAsync(s => s.StudentId == createSubmissionDto.StudentId);
        
        if (student == null)
            throw new BadHttpRequestException("Student not found");

        if (!student.IsActive)
            throw new BadHttpRequestException("Student must be active");
        
        var assignment = await _db.Assignments.AsNoTracking().Include(a => a.Submissions).FirstOrDefaultAsync(a => a.AssignmentId == createSubmissionDto.AssignmentId);
        if (assignment is not { IsPublished: true })
            throw new BadHttpRequestException("The assignment must exist and be published.");

        if (!student.Enrollments.Any(e => e.CourseId == assignment.CourseId &&
                                          e.Status is "Active" or "Completed"))
            throw new BadHttpRequestException("The student must be enrolled in the course that owns the assignment with status Active or Completed.");
        
        if (student.Submissions.Any(s => s.AssignmentId == assignment.AssignmentId))
            throw new BadHttpRequestException("The same student cannot submit the same assignment twice.");
            
        var status = DateTime.Now > assignment.DueDate ? "Late" : "Submitted";

        var submission = new Submission
        {
            AssignmentId = createSubmissionDto.AssignmentId,
            RepositoryUrl = createSubmissionDto.RepositoryUrl,
            StudentId = createSubmissionDto.StudentId,
            Status = status,
            SubmittedAt = DateTime.Now
        };
        _db.Submissions.Add(submission);

        await _db.SaveChangesAsync();

        return new SubmissionDto
        {
            AssignmentDto = new AssignmentDto()
            {
                AssignmentId  =  assignment.AssignmentId,
                IsPublished = assignment.IsPublished,
                MaxPoints = assignment.MaxPoints,
                DueDate = assignment.DueDate,
                Title = assignment.Title,
                SubmissionCount = assignment.Submissions.Count
            },
            RepositoryUrl = createSubmissionDto.RepositoryUrl,
            Status = status,
            SubmissionId = submission.SubmissionId,
            StudentDto = new StudentDto
            {
                IsActive = student.IsActive,
                StudentId = student.StudentId,
                Email = student.Email,
                EnrollmentDate = student.EnrollmentDate,
                FirstName = student.FirstName,
                IndexNumber = student.IndexNumber,
                LastName = student.LastName
            }
        };
    }
    
    public async Task<SubmissionDto> GradeSubmission(int id, GradeSubmissionDto gradeSubmissionDto)
    {
        var submission = _db.Submissions
            .Include(s => s.Assignment)
            .ThenInclude(a => a.Submissions)
            .Include(s => s.Student)
            .FirstOrDefault(s => s.SubmissionId == id);
        if (submission == null)
            throw new NotFoundException("Submission not found");

        if (gradeSubmissionDto.Score > submission.Assignment.MaxPoints)
            throw new BadHttpRequestException("The score cannot be higher than the assignment's MaxPoints.");
        
        submission.Score = gradeSubmissionDto.Score;
        submission.Feedback = gradeSubmissionDto.Feedback;
        submission.Status = "Graded";
        await _db.SaveChangesAsync();

        return new SubmissionDto
        {
            AssignmentDto = new AssignmentDto()
            {
                AssignmentId  =  submission.Assignment.AssignmentId,
                IsPublished = submission.Assignment.IsPublished,
                MaxPoints = submission.Assignment.MaxPoints,
                DueDate = submission.Assignment.DueDate,
                Title = submission.Assignment.Title,
                SubmissionCount = submission.Assignment.Submissions.Count
            },
            RepositoryUrl = submission.RepositoryUrl,
            Status = submission.Status,
            SubmissionId = submission.SubmissionId,
            StudentDto = new StudentDto
            {
                IsActive = submission.Student.IsActive,
                StudentId = submission.Student.StudentId,
                Email = submission.Student.Email,
                EnrollmentDate = submission.Student.EnrollmentDate,
                FirstName = submission.Student.FirstName,
                IndexNumber = submission.Student.IndexNumber,
                LastName = submission.Student.LastName
            },
            Feedback = submission.Feedback,
            Score = submission.Score
        };
    }

    public async Task DeleteSubmission(int id)
    {
        var submission = await _db.Submissions.AsNoTracking().FirstOrDefaultAsync(s => s.SubmissionId == id);
        if (submission == null)
            throw new NotFoundException("Submission not found");
        if (submission.Status == "Graded")
            throw new BadHttpRequestException("A graded submission cannot be deleted.");
        _db.Submissions.Remove(submission);
        await _db.SaveChangesAsync();
    }
    
    
}