namespace Alumni.Business.DTOs.Alumni;

public class AlumniProfileDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? Bio { get; set; }
    public List<EmploymentHistoryDto> EmploymentHistories { get; set; } = new();
}

public class CreateAlumniProfileRequest
{
    public int UserId { get; set; }
    public int DepartmentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
    public string? PhoneNumber { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? Bio { get; set; }
}

public class UpdateAlumniProfileRequest
{
    public string? PhoneNumber { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? Bio { get; set; }
}

public class EmploymentHistoryDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
}

public class AddEmploymentHistoryRequest
{
    public int AlumniProfileId { get; set; }
    public int CompanyId { get; set; }
    public string Position { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
}

