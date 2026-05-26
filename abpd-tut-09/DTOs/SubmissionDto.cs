using abpd_tut_09.Models;

namespace abpd_tut_09.DTOs;

public class SubmissionDto
{
    public int SubmissionId { get; set; }

    public Student Student { get; set; } = null!;
    
    public Assignment Assignment { get; set; } = null!;
    
    public string RepositoryUrl { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int? Score { get; set; }
    
    public string? Feedback { get; set; }
}