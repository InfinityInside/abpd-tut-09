using System.ComponentModel.DataAnnotations;

namespace abpd_tut_09.DTOs;

public class CreateSubmissionDto
{
    [Required]
    public int AssignmentId { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public string RepositoryUrl { get; set; } = null!;

}