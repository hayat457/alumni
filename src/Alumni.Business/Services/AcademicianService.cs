using Alumni.Business.DTOs.Academician;
using Alumni.Domain.Entities;
using Alumni.Domain.Interfaces;

namespace Alumni.Business.Services;

public class AcademicianService
{
    private readonly IAcademicianRepository _repository;

    public AcademicianService(IAcademicianRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AcademicianDto>> GetAllAsync()
    {
        var list = await _repository.GetAllAsync();
        return list.Select(MapToDto);
    }

    public async Task<AcademicianDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<IEnumerable<AcademicianDto>> GetByDepartmentAsync(int departmentId)
    {
        var list = await _repository.GetByDepartmentAsync(departmentId);
        return list.Select(MapToDto);
    }

    public async Task<AcademicianDto> CreateAsync(CreateAcademicianRequest request)
    {
        var entity = new Academician
        {
            UserId = request.UserId,
            DepartmentId = request.DepartmentId,
            Title = request.Title,
            PhoneNumber = request.PhoneNumber,
            OfficeRoom = request.OfficeRoom,
            ResearchArea = request.ResearchArea,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _repository.AddAsync(entity);
        return MapToDto(entity);
    }

    private static AcademicianDto MapToDto(Academician a) => new()
    {
        Id = a.Id,
        UserId = a.UserId,
        FullName = a.User != null ? $"{a.User.FirstName} {a.User.LastName}" : string.Empty,
        Email = a.User?.Email ?? string.Empty,
        Title = a.Title,
        DepartmentName = a.Department?.Name ?? string.Empty,
        PhoneNumber = a.PhoneNumber,
        OfficeRoom = a.OfficeRoom,
        ResearchArea = a.ResearchArea
    };
}

