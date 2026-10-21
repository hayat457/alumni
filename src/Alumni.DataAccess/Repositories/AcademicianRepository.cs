using Alumni.DataAccess.Context;
using Alumni.Domain.Entities;
using Alumni.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alumni.DataAccess.Repositories;

public class AcademicianRepository : Repository<Academician>, IAcademicianRepository
{
    public AcademicianRepository(AlumniDbContext context) : base(context) { }

    public async Task<Academician?> GetByUserIdAsync(int userId) =>
        await _dbSet
            .Include(a => a.User)
            .Include(a => a.Department)
            .FirstOrDefaultAsync(a => a.UserId == userId);

    public async Task<IEnumerable<Academician>> GetByDepartmentAsync(int departmentId) =>
        await _dbSet
            .Include(a => a.User)
            .Where(a => a.DepartmentId == departmentId && a.IsActive)
            .ToListAsync();
}

