using Alumni.Domain.Entities;

namespace Alumni.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetWithProfileAsync(int userId);
}

