using abpd_tut_09.Models;

namespace abpd_tut_09.DTOs;

public class SubmissionDto
{
    public int SubmissionId { get; set; }

    public StudentDto StudentDto { get; set; } = null!;
    
    public AssignmentDto AssignmentDto { get; set; } = null!;
    
    public string RepositoryUrl { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int? Score { get; set; }
    
    public string? Feedback { get; set; }
}

public class StudentDto
{
    public int StudentId { get; set; }

    public string IndexNumber { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateOnly EnrollmentDate { get; set; }

    public bool IsActive { get; set; }

}
