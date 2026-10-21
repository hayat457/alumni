using System.Collections.Concurrent;
using Alumni.Business.DTOs.Announcement;

namespace Alumni.Business.Services;

/// <summary>
/// Duyuru yönetimini soyutlayan arayüz (Interface to manage announcements).
/// </summary>
public interface IAnnouncementStore
{
    IEnumerable<AnnouncementDto> GetAll();
    AnnouncementDto? GetById(int id);
    (AnnouncementDto? Announcement, string? Error) Create(CreateAnnouncementRequest request);
    (AnnouncementDto? Announcement, string? Error) Update(int id, UpdateAnnouncementRequest request);
    (AnnouncementDto? Announcement, string? Error) Patch(int id, PatchAnnouncementRequest request);
    bool Delete(int id);
}

/// <summary>
/// Veritabanı olmadan (without database yet) bellek içi duyuru yöneticisi.
/// </summary>
public class InMemoryAnnouncementStore : IAnnouncementStore
{
    private readonly ConcurrentDictionary<int, AnnouncementDto> _announcements = new();
    private int _nextId = 0;

    public InMemoryAnnouncementStore()
    {
        // Başlangıç için örnek duyuru verileri
        var sample1 = new AnnouncementDto
        {
            Id = Interlocked.Increment(ref _nextId),
            Title = "2026 Mezunlar Buluşması Tarihleri Açıklandı",
            Content = "Geleneksel mezunlar günü buluşmamız bu yıl 15 Temmuz yerleşkesinde gerçekleştirilecektir. Tüm mezunlarımız davetlidir.",
            Author = "Rektörlük",
            Category = "Etkinlik",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };
        _announcements.TryAdd(sample1.Id, sample1);

        var sample2 = new AnnouncementDto
        {
            Id = Interlocked.Increment(ref _nextId),
            Title = "Kariyer Zirvesi ve Şirket Mülakat Günleri",
            Content = "Önde gelen teknoloji şirketlerinin katılımıyla online kariyer günleri başlıyor. Özgeçmişlerinizi güncellemeyi unutmayın.",
            Author = "Kariyer Merkezi",
            Category = "Kariyer",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        _announcements.TryAdd(sample2.Id, sample2);
    }

    public IEnumerable<AnnouncementDto> GetAll()
    {
        return _announcements.Values.OrderByDescending(a => a.CreatedAt);
    }

    public AnnouncementDto? GetById(int id)
    {
        _announcements.TryGetValue(id, out var announcement);
        return announcement;
    }

    public (AnnouncementDto? Announcement, string? Error) Create(CreateAnnouncementRequest request)
    {
        if (request == null)
            return (null, "İstek gövdesi boş olamaz.");

        if (string.IsNullOrWhiteSpace(request.Title))
            return (null, "Başlık alanı zorunludur.");

        if (string.IsNullOrWhiteSpace(request.Content))
            return (null, "İçerik alanı zorunludur.");

        var newAnnouncement = new AnnouncementDto
        {
            Id = Interlocked.Increment(ref _nextId),
            Title = request.Title.Trim(),
            Content = request.Content.Trim(),
            Author = !string.IsNullOrWhiteSpace(request.Author) ? request.Author.Trim() : "Yönetim",
            Category = !string.IsNullOrWhiteSpace(request.Category) ? request.Category.Trim() : "Genel",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _announcements.TryAdd(newAnnouncement.Id, newAnnouncement);
        return (newAnnouncement, null);
    }

    public (AnnouncementDto? Announcement, string? Error) Update(int id, UpdateAnnouncementRequest request)
    {
        if (request == null)
            return (null, "İstek gövdesi boş olamaz.");

        if (!_announcements.TryGetValue(id, out var existing))
            return (null, $"ID'si #{id} olan duyuru bulunamadı.");

        if (string.IsNullOrWhiteSpace(request.Title))
            return (null, "Başlık alanı zorunludur.");

        if (string.IsNullOrWhiteSpace(request.Content))
            return (null, "İçerik alanı zorunludur.");

        var updated = new AnnouncementDto
        {
            Id = id,
            Title = request.Title.Trim(),
            Content = request.Content.Trim(),
            Author = !string.IsNullOrWhiteSpace(request.Author) ? request.Author.Trim() : existing.Author,
            Category = !string.IsNullOrWhiteSpace(request.Category) ? request.Category.Trim() : existing.Category,
            IsActive = request.IsActive,
            CreatedAt = existing.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        _announcements[id] = updated;
        return (updated, null);
    }

    public (AnnouncementDto? Announcement, string? Error) Patch(int id, PatchAnnouncementRequest request)
    {
        if (request == null)
            return (null, "İstek gövdesi boş olamaz.");

        if (!_announcements.TryGetValue(id, out var existing))
            return (null, $"ID'si #{id} olan duyuru bulunamadı.");

        var patched = new AnnouncementDto
        {
            Id = id,
            Title = !string.IsNullOrWhiteSpace(request.Title) ? request.Title.Trim() : existing.Title,
            Content = !string.IsNullOrWhiteSpace(request.Content) ? request.Content.Trim() : existing.Content,
            Author = !string.IsNullOrWhiteSpace(request.Author) ? request.Author.Trim() : existing.Author,
            Category = !string.IsNullOrWhiteSpace(request.Category) ? request.Category.Trim() : existing.Category,
            IsActive = request.IsActive.HasValue ? request.IsActive.Value : existing.IsActive,
            CreatedAt = existing.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        _announcements[id] = patched;
        return (patched, null);
    }

    public bool Delete(int id)
    {
        return _announcements.TryRemove(id, out _);
    }
}
