namespace Alumni.Domain.Entities;

/// <summary>
/// Üniversite bölümlerini temsil eder.
/// </summary>
public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<AlumniProfile> Alumni { get; set; } = new List<AlumniProfile>();
    public ICollection<Academician> Academicians { get; set; } = new List<Academician>();
}

