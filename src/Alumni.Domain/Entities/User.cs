using System.Collections.Concurrent;
using Alumni.Domain.Enums;

namespace Alumni.Domain.Entities;

/// <summary>
/// Sistemdeki kullanıcı modelidir. Veritabanı bağlantısı olmadan (without database connection)
/// bellek içi CRUD fonksiyonlarını da içerir.
/// </summary>
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Alumni;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties (EF Core için)
    public AlumniProfile? AlumniProfile { get; set; }
    public Academician? Academician { get; set; }

    // ─────────────────────────────────────────────────────────────────────────────
    // In-Memory CRUD Functions (Veritabanı bağlantısı olmadan bellek içi yönetim)
    // ─────────────────────────────────────────────────────────────────────────────
    private static readonly ConcurrentDictionary<int, User> _inMemoryUsers = new();
    private static int _idCounter = 0;

    static User()
    {
        // Başlangıç mock verileri
        var u1 = new User
        {
            Id = Interlocked.Increment(ref _idCounter),
            FirstName = "Ahmet",
            LastName = "Yılmaz",
            Email = "ahmet.yilmaz@example.com",
            Role = UserRole.Alumni,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _inMemoryUsers.TryAdd(u1.Id, u1);

        var u2 = new User
        {
            Id = Interlocked.Increment(ref _idCounter),
            FirstName = "Ayşe",
            LastName = "Kaya",
            Email = "ayse.kaya@example.com",
            Role = UserRole.Academician,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _inMemoryUsers.TryAdd(u2.Id, u2);
    }

    /// <summary>READ (ALL): Bellekteki tüm kullanıcıları listeler.</summary>
    public static IEnumerable<User> GetAll()
    {
        return _inMemoryUsers.Values.OrderBy(u => u.Id);
    }

    /// <summary>READ (ONE): ID ile kullanıcı getirir.</summary>
    public static User? GetById(int id)
    {
        _inMemoryUsers.TryGetValue(id, out var user);
        return user;
    }

    /// <summary>CREATE: Yeni kullanıcı ekler.</summary>
    public static (User? User, string? Error) Create(string firstName, string lastName, string email, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(firstName)) return (null, "Ad alanı zorunludur.");
        if (string.IsNullOrWhiteSpace(lastName)) return (null, "Soyad alanı zorunludur.");
        if (string.IsNullOrWhiteSpace(email)) return (null, "E-posta alanı zorunludur.");

        bool exists = _inMemoryUsers.Values.Any(u => string.Equals(u.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));
        if (exists) return (null, "Bu e-posta adresiyle kayıtlı bir kullanıcı zaten mevcut.");

        var newUser = new User
        {
            Id = Interlocked.Increment(ref _idCounter),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _inMemoryUsers.TryAdd(newUser.Id, newUser);
        return (newUser, null);
    }

    /// <summary>UPDATE: Kullanıcı bilgilerini günceller.</summary>
    public static (User? User, string? Error) Update(int id, string firstName, string lastName, string email, UserRole role, bool isActive)
    {
        if (!_inMemoryUsers.TryGetValue(id, out var existing))
            return (null, $"ID #{id} olan kullanıcı bulunamadı.");

        if (string.IsNullOrWhiteSpace(firstName)) return (null, "Ad alanı zorunludur.");
        if (string.IsNullOrWhiteSpace(lastName)) return (null, "Soyad alanı zorunludur.");
        if (string.IsNullOrWhiteSpace(email)) return (null, "E-posta alanı zorunludur.");

        bool emailTaken = _inMemoryUsers.Values.Any(u => u.Id != id && string.Equals(u.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));
        if (emailTaken) return (null, "Bu e-posta adresi başka bir kullanıcı tarafından kullanılıyor.");

        existing.FirstName = firstName.Trim();
        existing.LastName = lastName.Trim();
        existing.Email = email.Trim().ToLowerInvariant();
        existing.Role = role;
        existing.IsActive = isActive;
        existing.UpdatedAt = DateTime.UtcNow;

        _inMemoryUsers[id] = existing;
        return (existing, null);
    }

    /// <summary>DELETE: Kullanıcıyı bellekten siler.</summary>
    public static bool Delete(int id)
    {
        return _inMemoryUsers.TryRemove(id, out _);
    }
}
