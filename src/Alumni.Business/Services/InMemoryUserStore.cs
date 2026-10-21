using System.Collections.Concurrent;
using Alumni.Business.DTOs.User;

namespace Alumni.Business.Services;

public interface IInMemoryUserStore
{
    IEnumerable<UserDto> GetAll();
    UserDto? GetById(int id);
    (UserDto? User, string? Error) Create(CreateUserRequest request);
    (UserDto? User, string? Error) Update(int id, UpdateUserRequest request);
    (UserDto? User, string? Error) Patch(int id, PatchUserRequest request);
    bool Delete(int id);
}

public class InMemoryUserStore : IInMemoryUserStore
{
    private readonly ConcurrentDictionary<int, UserDto> _users = new();
    private int _nextId = 0;

    public InMemoryUserStore()
    {
        // Başlangıç için örnek mock veriler
        var sample1 = new UserDto
        {
            Id = Interlocked.Increment(ref _nextId),
            FirstName = "Ahmet",
            LastName = "Yılmaz",
            Email = "ahmet.yilmaz@example.com",
            Role = "Alumni",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _users.TryAdd(sample1.Id, sample1);

        var sample2 = new UserDto
        {
            Id = Interlocked.Increment(ref _nextId),
            FirstName = "Ayşe",
            LastName = "Kaya",
            Email = "ayse.kaya@example.com",
            Role = "Academician",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _users.TryAdd(sample2.Id, sample2);
    }

    public IEnumerable<UserDto> GetAll()
    {
        return _users.Values.OrderBy(u => u.Id);
    }

    public UserDto? GetById(int id)
    {
        _users.TryGetValue(id, out var user);
        return user;
    }

    public (UserDto? User, string? Error) Create(CreateUserRequest request)
    {
        if (request == null)
            return (null, "İstek gövdesi boş olamaz.");

        if (string.IsNullOrWhiteSpace(request.FirstName))
            return (null, "Ad alanı zorunludur.");

        if (string.IsNullOrWhiteSpace(request.LastName))
            return (null, "Soyad alanı zorunludur.");

        if (string.IsNullOrWhiteSpace(request.Email))
            return (null, "E-posta alanı zorunludur.");

        bool emailExists = _users.Values.Any(u =>
            string.Equals(u.Email, request.Email.Trim(), StringComparison.OrdinalIgnoreCase));

        if (emailExists)
            return (null, "Bu e-posta adresiyle kayıtlı bir kullanıcı zaten mevcut.");

        var newUser = new UserDto
        {
            Id = Interlocked.Increment(ref _nextId),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Role = request.Role.ToString(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _users.TryAdd(newUser.Id, newUser);
        return (newUser, null);
    }

    public (UserDto? User, string? Error) Update(int id, UpdateUserRequest request)
    {
        if (request == null)
            return (null, "İstek gövdesi boş olamaz.");

        if (!_users.TryGetValue(id, out var existingUser))
            return (null, $"ID'si {id} olan kullanıcı bulunamadı.");

        if (string.IsNullOrWhiteSpace(request.FirstName))
            return (null, "Ad alanı zorunludur.");

        if (string.IsNullOrWhiteSpace(request.LastName))
            return (null, "Soyad alanı zorunludur.");

        if (string.IsNullOrWhiteSpace(request.Email))
            return (null, "E-posta alanı zorunludur.");

        bool emailExists = _users.Values.Any(u =>
            u.Id != id && string.Equals(u.Email, request.Email.Trim(), StringComparison.OrdinalIgnoreCase));

        if (emailExists)
            return (null, "Bu e-posta adresi başka bir kullanıcı tarafından kullanılıyor.");

        var updatedUser = new UserDto
        {
            Id = id,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Role = request.Role.ToString(),
            IsActive = request.IsActive,
            CreatedAt = existingUser.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        _users[id] = updatedUser;
        return (updatedUser, null);
    }

    public (UserDto? User, string? Error) Patch(int id, PatchUserRequest request)
    {
        if (request == null)
            return (null, "İstek gövdesi boş olamaz.");

        if (!_users.TryGetValue(id, out var existingUser))
            return (null, $"ID'si {id} olan kullanıcı bulunamadı.");

        string newEmail = existingUser.Email;
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var trimmedEmail = request.Email.Trim().ToLowerInvariant();
            bool emailExists = _users.Values.Any(u =>
                u.Id != id && string.Equals(u.Email, trimmedEmail, StringComparison.OrdinalIgnoreCase));

            if (emailExists)
                return (null, "Bu e-posta adresi başka bir kullanıcı tarafından kullanılıyor.");

            newEmail = trimmedEmail;
        }

        var patchedUser = new UserDto
        {
            Id = id,
            FirstName = !string.IsNullOrWhiteSpace(request.FirstName) ? request.FirstName.Trim() : existingUser.FirstName,
            LastName = !string.IsNullOrWhiteSpace(request.LastName) ? request.LastName.Trim() : existingUser.LastName,
            Email = newEmail,
            Role = request.Role.HasValue ? request.Role.Value.ToString() : existingUser.Role,
            IsActive = request.IsActive.HasValue ? request.IsActive.Value : existingUser.IsActive,
            CreatedAt = existingUser.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        _users[id] = patchedUser;
        return (patchedUser, null);
    }

    public bool Delete(int id)
    {
        return _users.TryRemove(id, out _);
    }
}
