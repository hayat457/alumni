namespace Alumni.Domain.Entities;

/// <summary>
/// İş ilanlarına yapılan başvuruları temsil eden model (Job Application Model).
/// </summary>
public class JobApplication
{
    public int Id { get; set; }
    
    // İlişkiler (Relationships)
    public int JobPostingId { get; set; }
    public JobPosting JobPosting { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string? ApplicantName { get; set; }
    public string? ApplicantEmail { get; set; }
    public string? CoverLetter { get; set; }
    public string? ResumeUrl { get; set; }
    public string Status { get; set; } = "Beklemede"; // Beklemede, İncelendi, Mülakat, Kabul, Red
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
}
