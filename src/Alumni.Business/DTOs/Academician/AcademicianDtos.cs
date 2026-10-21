namespace Alumni.Business.DTOs.Academician;

public class AcademicianDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? OfficeRoom { get; set; }
    public string? ResearchArea { get; set; }
}

public class CreateAcademicianRequest
{
    public int UserId { get; set; }
    public int DepartmentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? OfficeRoom { get; set; }
    public string? ResearchArea { get; set; }
}

