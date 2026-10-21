namespace Alumni.Domain.Entities;

/// <summary>
/// Bir mezunun bir şirketteki çalışma geçmişini tutar.
/// </summary>
public class EmploymentHistory
{
    public int Id { get; set; }
    public int AlumniProfileId { get; set; }
    public int CompanyId { get; set; }
    public string Position { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; } = false;
    public string? Description { get; set; }

    // Navigation properties
    public AlumniProfile AlumniProfile { get; set; } = null!;
    public Company Company { get; set; } = null!;
}

