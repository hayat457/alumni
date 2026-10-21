using Alumni.Business.DTOs.User;
using Alumni.Domain.Entities;
using Alumni.Domain.Enums;

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
    private static UserDto MapToDto(User u) => new()
    {
        Id = u.Id,
        FirstName = u.FirstName,
        LastName = u.LastName,
        Email = u.Email,
        Role = u.Role.ToString(),
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt
    };

    public IEnumerable<UserDto> GetAll()
    {
        return User.GetAll().Select(MapToDto);
    }

    public UserDto? GetById(int id)
    {
        var u = User.GetById(id);
        return u != null ? MapToDto(u) : null;
    }

    public (UserDto? User, string? Error) Create(CreateUserRequest request)
    {
        if (request == null)
            return (null, "İstek gövdesi boş olamaz.");

        var (newUser, error) = User.Create(request.FirstName, request.LastName, request.Email, request.Role);
        return (newUser != null ? MapToDto(newUser) : null, error);
    }

    public (UserDto? User, string? Error) Update(int id, UpdateUserRequest request)
    {
        if (request == null)
            return (null, "İstek gövdesi boş olamaz.");

        var (updated, error) = User.Update(id, request.FirstName, request.LastName, request.Email, request.Role, request.IsActive);
        return (updated != null ? MapToDto(updated) : null, error);
    }

    public (UserDto? User, string? Error) Patch(int id, PatchUserRequest request)
    {
        if (request == null)
            return (null, "İstek gövdesi boş olamaz.");

        var existing = User.GetById(id);
        if (existing == null)
            return (null, $"ID #{id} olan kullanıcı bulunamadı.");

        var firstName = !string.IsNullOrWhiteSpace(request.FirstName) ? request.FirstName : existing.FirstName;
        var lastName = !string.IsNullOrWhiteSpace(request.LastName) ? request.LastName : existing.LastName;
        var email = !string.IsNullOrWhiteSpace(request.Email) ? request.Email : existing.Email;
        var role = request.Role ?? existing.Role;
        var isActive = request.IsActive ?? existing.IsActive;

        var (updated, error) = User.Update(id, firstName, lastName, email, role, isActive);
        return (updated != null ? MapToDto(updated) : null, error);
    }

    public bool Delete(int id)
    {
        return User.Delete(id);
    }
}
