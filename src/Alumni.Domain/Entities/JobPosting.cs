using System.ComponentModel.DataAnnotations;

namespace Alumni.Domain.Entities;

/// <summary>
/// Şirketlerin açtığı iş/staj ilanlarını temsil eden model (Job Posting Model).
/// </summary>
public class JobPosting
{
    public int Id { get; set; }

    [Required(ErrorMessage = "İlan başlığı (title) zorunludur.")]
    [StringLength(120, ErrorMessage = "İlan başlığı (title) en fazla 120 karakter olabilir.")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Requirements { get; set; }

    // City / Location opsiyoneldir
    public string? Location { get; set; } = "İstanbul";

    public string EmploymentType { get; set; } = "Tam Zamanlı"; // Tam Zamanlı, Yarı Zamanlı, Staj, Uzaktan
    public string? SalaryRange { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosingDate { get; set; }

    // İlişkiler (Relationships) - Company zorunludur
    [Required(ErrorMessage = "Şirket seçimi (company) zorunludur.")]
    [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir şirket (company) seçilmelidir.")]
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}
