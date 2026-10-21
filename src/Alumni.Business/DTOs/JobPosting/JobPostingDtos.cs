namespace Alumni.Business.DTOs.JobPosting;

public class CreateJobPostingRequest
{
    public int CompanyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Requirements { get; set; }
    public string Location { get; set; } = "İstanbul";
    public string EmploymentType { get; set; } = "Tam Zamanlı";
    public string? SalaryRange { get; set; }
    public DateTime? ClosingDate { get; set; }
}

public class UpdateJobPostingRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Requirements { get; set; }
    public string Location { get; set; } = "İstanbul";
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
    public string Description { get; set; } = string.Empty;
    public string? Requirements { get; set; }
    public string Location { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty;
    public string? SalaryRange { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosingDate { get; set; }
    public int ApplicationsCount { get; set; }
}
