using Alumni.DataAccess.Context;
using Alumni.Domain.Entities;
using Alumni.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alumni.DataAccess.Repositories;

public class AlumniRepository : Repository<AlumniProfile>, IAlumniRepository
{
    public AlumniRepository(AlumniDbContext context) : base(context) { }

    public async Task<AlumniProfile?> GetByUserIdAsync(int userId) =>
        await _dbSet.FirstOrDefaultAsync(a => a.UserId == userId);

    public async Task<AlumniProfile?> GetWithDetailsAsync(int id) =>
        await _dbSet
            .Include(a => a.User)
            .Include(a => a.Department)
            .Include(a => a.EmploymentHistories).ThenInclude(eh => eh.Company)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<IEnumerable<AlumniProfile>> GetByDepartmentAsync(int departmentId) =>
        await _dbSet
            .Include(a => a.User)
            .Where(a => a.DepartmentId == departmentId)
            .ToListAsync();

    public async Task<IEnumerable<AlumniProfile>> GetByGraduationYearAsync(int year) =>
        await _dbSet
            .Include(a => a.User)
            .Where(a => a.GraduationYear == year)
            .ToListAsync();
}

