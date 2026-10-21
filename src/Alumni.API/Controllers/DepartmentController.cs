using Alumni.DataAccess.Context;
using Alumni.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Alumni.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentController : ControllerBase
{
    private readonly AlumniDbContext _context;

    public DepartmentController(AlumniDbContext context)
    {
        _context = context;
    }

    /// <summary>Tüm bölümleri listeler.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var departments = await _context.Departments
            .Where(d => d.IsActive)
            .Select(d => new { d.Id, d.Name, d.Code, d.Description })
            .ToListAsync();
        return Ok(departments);
    }

    /// <summary>ID'ye göre bölüm döner.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dept = await _context.Departments.FindAsync(id);
        return dept == null ? NotFound() : Ok(dept);
    }

    /// <summary>Yeni bölüm ekle. (Admin only)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] Department department)
    {
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
    }

    /// <summary>Bölüm güncelle. (Admin only)</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] Department updated)
    {
        var dept = await _context.Departments.FindAsync(id);
        if (dept == null) return NotFound();
        dept.Name = updated.Name;
        dept.Code = updated.Code;
        dept.Description = updated.Description;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Bölümü pasife al. (Admin only)</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var dept = await _context.Departments.FindAsync(id);
        if (dept == null) return NotFound();
        dept.IsActive = false;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

