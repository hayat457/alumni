namespace Alumni.Domain.Entities;

/// <summary>
/// Mezunların çalıştığı şirket bilgilerini tutar.
/// </summary>
public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Sector { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool IsVerified { get; set; } = false;

    // Navigation properties
    public ICollection<EmploymentHistory> EmploymentHistories { get; set; } = new List<EmploymentHistory>();
}

