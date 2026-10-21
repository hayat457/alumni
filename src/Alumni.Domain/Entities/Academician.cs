namespace Alumni.Domain.Entities;

/// <summary>
/// Akademisyen profil bilgilerini tutar.
/// </summary>
public class Academician
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DepartmentId { get; set; }
    public string Title { get; set; } = string.Empty;  // Prof., Doç., Dr., vb.
    public string? PhoneNumber { get; set; }
    public string? OfficeRoom { get; set; }
    public string? ResearchArea { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public Department Department { get; set; } = null!;
}

