using Alumni.Business.DTOs.Academician;
using Alumni.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AcademicianController : ControllerBase
{
    private readonly AcademicianService _academicianService;

    public AcademicianController(AcademicianService academicianService)
    {
        _academicianService = academicianService;
    }

    /// <summary>Tüm akademisyenleri listeler.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _academicianService.GetAllAsync();
        return Ok(result);
    }

    /// <summary>ID'ye göre akademisyen döner.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _academicianService.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>Bölüme göre akademisyen listeler.</summary>
    [HttpGet("department/{departmentId:int}")]
    public async Task<IActionResult> GetByDepartment(int departmentId)
    {
        var result = await _academicianService.GetByDepartmentAsync(departmentId);
        return Ok(result);
    }

    /// <summary>Yeni akademisyen kaydı oluştur. (Admin only)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateAcademicianRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _academicianService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}

