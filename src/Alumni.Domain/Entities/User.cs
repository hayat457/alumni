using Alumni.Domain.Enums;

namespace Alumni.Domain.Entities;

/// <summary>
/// Sistemdeki tüm kullanıcıların temel kimlik bilgilerini tutar.
/// </summary>
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public AlumniProfile? AlumniProfile { get; set; }
    public Academician? Academician { get; set; }
}

