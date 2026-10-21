using Alumni.Domain.Entities;

namespace Alumni.Domain.Interfaces;

public interface IAcademicianRepository : IRepository<Academician>
{
    Task<Academician?> GetByUserIdAsync(int userId);
    Task<IEnumerable<Academician>> GetByDepartmentAsync(int departmentId);
}

