using Alumni.Domain.Entities;

namespace Alumni.Domain.Interfaces;

public interface IEmploymentHistoryRepository : IRepository<EmploymentHistory>
{
    Task<IEnumerable<EmploymentHistory>> GetByAlumniIdAsync(int alumniProfileId);
    Task<IEnumerable<EmploymentHistory>> GetCurrentJobsAsync();
}

