namespace Alumni.Domain.Entities;

/// <summary>
/// Mezun öğrencilerin akademik ve iletişim bilgilerini tutar.
/// </summary>
public class AlumniProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DepartmentId { get; set; }

    /// <summary>SIS entegrasyonu için öğrenci numarası.</summary>
    public string StudentNumber { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
    public string? PhoneNumber { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public Department Department { get; set; } = null!;
    public ICollection<EmploymentHistory> EmploymentHistories { get; set; } = new List<EmploymentHistory>();
}

