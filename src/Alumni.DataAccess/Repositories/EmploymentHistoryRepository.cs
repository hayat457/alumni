using Alumni.DataAccess.Context;
using Alumni.Domain.Entities;
using Alumni.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alumni.DataAccess.Repositories;

public class EmploymentHistoryRepository : Repository<EmploymentHistory>, IEmploymentHistoryRepository
{
    public EmploymentHistoryRepository(AlumniDbContext context) : base(context) { }

    public async Task<IEnumerable<EmploymentHistory>> GetByAlumniIdAsync(int alumniProfileId) =>
        await _dbSet
            .Include(eh => eh.Company)
            .Where(eh => eh.AlumniProfileId == alumniProfileId)
            .OrderByDescending(eh => eh.StartDate)
            .ToListAsync();

    public async Task<IEnumerable<EmploymentHistory>> GetCurrentJobsAsync() =>
        await _dbSet
            .Include(eh => eh.AlumniProfile).ThenInclude(a => a.User)
            .Include(eh => eh.Company)
            .Where(eh => eh.IsCurrent)
            .ToListAsync();
}

