namespace Alumni.Business.DTOs.Announcement;

public class CreateAnnouncementRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Category { get; set; } = "Genel";
}

public class UpdateAnnouncementRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Category { get; set; } = "Genel";
    public bool IsActive { get; set; } = true;
}

public class PatchAnnouncementRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Author { get; set; }
    public string? Category { get; set; }
    public bool? IsActive { get; set; }
}

public class AnnouncementDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
