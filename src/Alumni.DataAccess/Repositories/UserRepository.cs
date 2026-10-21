using Alumni.DataAccess.Context;
using Alumni.Domain.Entities;
using Alumni.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alumni.DataAccess.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AlumniDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email) =>
        await _dbSet.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetWithProfileAsync(int userId) =>
        await _dbSet
            .Include(u => u.AlumniProfile).ThenInclude(a => a!.Department)
            .Include(u => u.Academician).ThenInclude(a => a!.Department)
            .FirstOrDefaultAsync(u => u.Id == userId);
}

