namespace Alumni.Domain.Entities;

/// <summary>
/// Şirketlerin açtığı iş/staj ilanlarını temsil eden model (Job Posting Model).
/// </summary>
public class JobPosting
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Requirements { get; set; }
    public string Location { get; set; } = "İstanbul";
    public string EmploymentType { get; set; } = "Tam Zamanlı"; // Tam Zamanlı, Yarı Zamanlı, Staj, Uzaktan
    public string? SalaryRange { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosingDate { get; set; }

    // İlişkiler (Relationships)
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}
