using Alumni.Business.DTOs.Alumni;
using Alumni.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlumniController : ControllerBase
{
    private readonly AlumniService _alumniService;

    public AlumniController(AlumniService alumniService)
    {
        _alumniService = alumniService;
    }

    /// <summary>Tüm mezun profillerini listeler. (Admin, Academician)</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Academician")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _alumniService.GetAllAsync();
        return Ok(result);
    }

    /// <summary>Belirli bir mezun profilini döner.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _alumniService.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>Bölüme göre mezun filtrele.</summary>
    [HttpGet("department/{departmentId:int}")]
    [Authorize(Roles = "Admin,Academician")]
    public async Task<IActionResult> GetByDepartment(int departmentId)
    {
        var result = await _alumniService.GetByDepartmentAsync(departmentId);
        return Ok(result);
    }

    /// <summary>Mezuniyet yılına göre filtrele.</summary>
    [HttpGet("year/{year:int}")]
    [Authorize(Roles = "Admin,Academician")]
    public async Task<IActionResult> GetByYear(int year)
    {
        var result = await _alumniService.GetByGraduationYearAsync(year);
        return Ok(result);
    }

    /// <summary>Yeni mezun profili oluştur.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Alumni")]
    public async Task<IActionResult> Create([FromBody] CreateAlumniProfileRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _alumniService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Mezun profilini güncelle.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Alumni")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAlumniProfileRequest request)
    {
        var success = await _alumniService.UpdateAsync(id, request);
        return success ? NoContent() : NotFound();
    }

    /// <summary>Mezun profilini sil. (Yalnızca Admin)</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _alumniService.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }

    /// <summary>Mezuna iş geçmişi ekle.</summary>
    [HttpPost("{id:int}/employment")]
    [Authorize(Roles = "Admin,Alumni")]
    public async Task<IActionResult> AddEmployment(int id, [FromBody] AddEmploymentHistoryRequest request)
    {
        request.AlumniProfileId = id;
        var result = await _alumniService.AddEmploymentHistoryAsync(request);
        return Created(string.Empty, result);
    }
}

