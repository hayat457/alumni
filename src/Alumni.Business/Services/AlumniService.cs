using Alumni.Business.DTOs.Alumni;
using Alumni.Domain.Entities;
using Alumni.Domain.Interfaces;

namespace Alumni.Business.Services;

public class AlumniService
{
    private readonly IAlumniRepository _alumniRepository;
    private readonly IEmploymentHistoryRepository _employmentHistoryRepository;

    public AlumniService(
        IAlumniRepository alumniRepository,
        IEmploymentHistoryRepository employmentHistoryRepository)
    {
        _alumniRepository = alumniRepository;
        _employmentHistoryRepository = employmentHistoryRepository;
    }

    public async Task<IEnumerable<AlumniProfileDto>> GetAllAsync()
    {
        var profiles = await _alumniRepository.GetAllAsync();
        // Not: Production'da Include ile User/Department eager loading yapılmalı
        return profiles.Select(MapToDto);
    }

    public async Task<AlumniProfileDto?> GetByIdAsync(int id)
    {
        var profile = await _alumniRepository.GetWithDetailsAsync(id);
        return profile == null ? null : MapToDetailedDto(profile);
    }

    public async Task<AlumniProfileDto?> GetByUserIdAsync(int userId)
    {
        var profile = await _alumniRepository.GetByUserIdAsync(userId);
        return profile == null ? null : MapToDto(profile);
    }

    public async Task<IEnumerable<AlumniProfileDto>> GetByDepartmentAsync(int departmentId)
    {
        var profiles = await _alumniRepository.GetByDepartmentAsync(departmentId);
        return profiles.Select(MapToDto);
    }

    public async Task<IEnumerable<AlumniProfileDto>> GetByGraduationYearAsync(int year)
    {
        var profiles = await _alumniRepository.GetByGraduationYearAsync(year);
        return profiles.Select(MapToDto);
    }

    public async Task<AlumniProfileDto> CreateAsync(CreateAlumniProfileRequest request)
    {
        var profile = new AlumniProfile
        {
            UserId = request.UserId,
            DepartmentId = request.DepartmentId,
            StudentNumber = request.StudentNumber,
            GraduationYear = request.GraduationYear,
            PhoneNumber = request.PhoneNumber,
            LinkedInUrl = request.LinkedInUrl,
            Bio = request.Bio,
            CreatedAt = DateTime.UtcNow
        };
        await _alumniRepository.AddAsync(profile);
        return MapToDto(profile);
    }

    public async Task<bool> UpdateAsync(int id, UpdateAlumniProfileRequest request)
    {
        var profile = await _alumniRepository.GetByIdAsync(id);
        if (profile == null) return false;

        profile.PhoneNumber = request.PhoneNumber ?? profile.PhoneNumber;
        profile.LinkedInUrl = request.LinkedInUrl ?? profile.LinkedInUrl;
        profile.Bio = request.Bio ?? profile.Bio;

        await _alumniRepository.UpdateAsync(profile);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var profile = await _alumniRepository.GetByIdAsync(id);
        if (profile == null) return false;
        await _alumniRepository.DeleteAsync(profile);
        return true;
    }

    public async Task<EmploymentHistoryDto> AddEmploymentHistoryAsync(AddEmploymentHistoryRequest request)
    {
        var history = new EmploymentHistory
        {
            AlumniProfileId = request.AlumniProfileId,
            CompanyId = request.CompanyId,
            Position = request.Position,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsCurrent = request.IsCurrent,
            Description = request.Description
        };
        await _employmentHistoryRepository.AddAsync(history);
        return new EmploymentHistoryDto
        {
            Id = history.Id,
            Position = history.Position,
            StartDate = history.StartDate,
            EndDate = history.EndDate,
            IsCurrent = history.IsCurrent,
            Description = history.Description
        };
    }

    private static AlumniProfileDto MapToDto(AlumniProfile p) => new()
    {
        Id = p.Id,
        UserId = p.UserId,
        FullName = p.User != null ? $"{p.User.FirstName} {p.User.LastName}" : string.Empty,
        Email = p.User?.Email ?? string.Empty,
        StudentNumber = p.StudentNumber,
        GraduationYear = p.GraduationYear,
        DepartmentName = p.Department?.Name ?? string.Empty,
        PhoneNumber = p.PhoneNumber,
        LinkedInUrl = p.LinkedInUrl,
        Bio = p.Bio
    };

    private static AlumniProfileDto MapToDetailedDto(AlumniProfile p)
    {
        var dto = MapToDto(p);
        dto.EmploymentHistories = p.EmploymentHistories.Select(eh => new EmploymentHistoryDto
        {
            Id = eh.Id,
            CompanyName = eh.Company?.Name ?? string.Empty,
            Position = eh.Position,
            StartDate = eh.StartDate,
            EndDate = eh.EndDate,
            IsCurrent = eh.IsCurrent,
            Description = eh.Description
        }).ToList();
        return dto;
    }
}

