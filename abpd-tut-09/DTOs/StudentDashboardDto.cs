namespace abpd_tut_09.DTOs;

public class StudentDashboardDto
{
    public int StudentId { get; set; }

    public string IndexNumber { get; set; } = null!;

    public string FullName { get; set; } = null!;
    
    public bool IsActive { get; set; }

    public ICollection<DashboardEnrollmentDto> Enrollments { get; set; } = [];

    public ICollection<DashboardSubmissionDto> Submissions { get; set; } = [];
}

public class DashboardEnrollmentDto
{
    public int EnrollmentId { get; set; }

    public int CourseId { get; set; }

    public DateOnly EnrolledAt { get; set; }

    public string Status { get; set; } = null!;

}

public class DashboardSubmissionDto
{
    public int SubmissionId { get; set; }

    public int AssignmentId { get; set; }

    public string RepositoryUrl { get; set; } = null!;

    public DateTime SubmittedAt { get; set; }

    public int? Score { get; set; }

    public string? Feedback { get; set; }

    public string Status { get; set; } = null!;

}