using Alumni.Domain.Entities;

namespace Alumni.Domain.Interfaces;

public interface IAlumniRepository : IRepository<AlumniProfile>
{
    Task<AlumniProfile?> GetByUserIdAsync(int userId);
    Task<AlumniProfile?> GetWithDetailsAsync(int id);
    Task<IEnumerable<AlumniProfile>> GetByDepartmentAsync(int departmentId);
    Task<IEnumerable<AlumniProfile>> GetByGraduationYearAsync(int year);
}

