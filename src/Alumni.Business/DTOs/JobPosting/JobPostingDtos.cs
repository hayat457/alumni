using System.ComponentModel.DataAnnotations;

namespace Alumni.Business.DTOs.JobPosting;

public class CreateJobPostingRequest
{
    [Required(ErrorMessage = "Şirket seçimi (company) zorunludur.")]
    [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir şirket (company) seçilmelidir.")]
    public int CompanyId { get; set; }

    [Required(ErrorMessage = "İlan başlığı (title) zorunludur.")]
    [StringLength(120, ErrorMessage = "İlan başlığı (title) en fazla 120 karakter olabilir.")]
    public string Title { get; set; } = string.Empty;

    // Opsiyonel alanlar
    public string? Description { get; set; }
    public string? Requirements { get; set; }
    public string? Location { get; set; } // Şehir / Lokasyon (Opsiyonel)
    public string? City { get; set; } // İsteğe bağlı şehir takma adı / city alanı
    public string EmploymentType { get; set; } = "Tam Zamanlı";
    public string? SalaryRange { get; set; }
    public DateTime? ClosingDate { get; set; }
}

public class UpdateJobPostingRequest
{
    [Required(ErrorMessage = "İlan başlığı (title) zorunludur.")]
    [StringLength(120, ErrorMessage = "İlan başlığı (title) en fazla 120 karakter olabilir.")]
    public string Title { get; set; } = string.Empty;

    // Opsiyonel alanlar
    public string? Description { get; set; }
    public string? Requirements { get; set; }
    public string? Location { get; set; }
    public string? City { get; set; }
    public string EmploymentType { get; set; } = "Tam Zamanlı";
    public string? SalaryRange { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? ClosingDate { get; set; }
}

public class JobPostingDto
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Requirements { get; set; }
    public string? Location { get; set; }
    public string EmploymentType { get; set; } = string.Empty;
    public string? SalaryRange { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosingDate { get; set; }
    public int ApplicationsCount { get; set; }
}
