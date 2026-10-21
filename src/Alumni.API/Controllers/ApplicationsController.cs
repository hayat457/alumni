using Alumni.Business.DTOs.JobApplication;
using Alumni.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Alumni.API.Controllers;

/// <summary>
/// Başvuru yönetimi REST API controller'ı (Doğrudan Rotalar / Direct Routes).
/// </summary>
[ApiController]
[Route("api/applications")]
public class ApplicationsController : ControllerBase
{
    private readonly AlumniDbContext _db;

    public ApplicationsController(AlumniDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// DIRECT ROUTE: Tüm iş başvurularını listeler.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<JobApplicationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? status)
    {
        var query = _db.JobApplications
            .Include(a => a.JobPosting).ThenInclude(j => j.Company)
            .Include(a => a.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(a => a.Status.ToLower() == status.Trim().ToLower());

        var list = await query.OrderByDescending(a => a.AppliedAt).Select(a => new JobApplicationDto
        {
            Id = a.Id,
            JobPostingId = a.JobPostingId,
            JobTitle = a.JobPosting.Title,
            CompanyName = a.JobPosting.Company.Name,
            UserId = a.UserId,
            ApplicantName = a.ApplicantName ?? $"{a.User.FirstName} {a.User.LastName}",
            ApplicantEmail = a.ApplicantEmail ?? a.User.Email,
            CoverLetter = a.CoverLetter,
            ResumeUrl = a.ResumeUrl,
            Status = a.Status,
            AppliedAt = a.AppliedAt
        }).ToListAsync();

        return Ok(list);
    }

    /// <summary>
    /// DIRECT ROUTE: ID'ye göre başvuru detayını getirir.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(JobApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var a = await _db.JobApplications
            .Include(x => x.JobPosting).ThenInclude(j => j.Company)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (a == null)
            return NotFound(new { message = $"ID'si #{id} olan başvuru bulunamadı." });

        return Ok(new JobApplicationDto
        {
            Id = a.Id,
            JobPostingId = a.JobPostingId,
            JobTitle = a.JobPosting.Title,
            CompanyName = a.JobPosting.Company.Name,
            UserId = a.UserId,
            ApplicantName = a.ApplicantName ?? $"{a.User.FirstName} {a.User.LastName}",
            ApplicantEmail = a.ApplicantEmail ?? a.User.Email,
            CoverLetter = a.CoverLetter,
            ResumeUrl = a.ResumeUrl,
            Status = a.Status,
            AppliedAt = a.AppliedAt
        });
    }

    /// <summary>
    /// DIRECT ROUTE: Başvuru durumunu günceller (Beklemede, İncelendi, Mülakat, Kabul, Red).
    /// </summary>
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateApplicationStatusRequest request)
    {
        var app = await _db.JobApplications.FindAsync(id);
        if (app == null)
            return NotFound(new { message = $"ID'si #{id} olan başvuru bulunamadı." });

        app.Status = request.Status;
        await _db.SaveChangesAsync();

        return Ok(new { message = $"Başvuru durumu '{request.Status}' olarak güncellendi.", id, status = request.Status });
    }

    /// <summary>
    /// DIRECT ROUTE: Başvuruyu geri çeker / siler.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var app = await _db.JobApplications.FindAsync(id);
        if (app == null)
            return NotFound(new { message = $"ID'si #{id} olan başvuru bulunamadı." });

        _db.JobApplications.Remove(app);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
