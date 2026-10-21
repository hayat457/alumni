namespace Alumni.Business.DTOs.JobApplication;

public class CreateJobApplicationRequest
{
    public int JobPostingId { get; set; }
    public int UserId { get; set; } = 1; // Varsayılan kullanıcı ID
    public string? ApplicantName { get; set; }
    public string? ApplicantEmail { get; set; }
    public string? CoverLetter { get; set; }
    public string? ResumeUrl { get; set; }
}

public class UpdateApplicationStatusRequest
{
    public string Status { get; set; } = "İncelendi"; // Beklemede, İncelendi, Mülakat, Kabul, Red
}

public class JobApplicationDto
{
    public int Id { get; set; }
    public int JobPostingId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    public string? CoverLetter { get; set; }
    public string? ResumeUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
}
