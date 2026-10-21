using Alumni.Business.DTOs.JobApplication;
using Alumni.Business.DTOs.JobPosting;
using Alumni.DataAccess.Context;
using Alumni.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Alumni.API.Controllers;

/// <summary>
/// İş ilanları controller'ı (Doğrudan ve İç İçe / Direct and Nested Routes).
/// </summary>
[ApiController]
public class JobPostingsController : ControllerBase
{
    private readonly AlumniDbContext _db;

    public JobPostingsController(AlumniDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// DIRECT ROUTE: Tüm aktif iş ilanlarını listeler (Arama/Filtreleme destekli).
    /// </summary>
    [HttpGet("api/job-postings")]
    [ProducesResponseType(typeof(IEnumerable<JobPostingDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int? companyId)
    {
        var query = _db.JobPostings.Include(j => j.Company).Include(j => j.Applications).AsQueryable();

        if (companyId.HasValue)
            query = query.Where(j => j.CompanyId == companyId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(j => j.Title.ToLower().Contains(s) || j.Location.ToLower().Contains(s) || j.Company.Name.ToLower().Contains(s));
        }

        var list = await query.OrderByDescending(j => j.CreatedAt).Select(j => new JobPostingDto
        {
            Id = j.Id,
            CompanyId = j.CompanyId,
            CompanyName = j.Company.Name,
            Title = j.Title,
            Description = j.Description,
            Requirements = j.Requirements,
            Location = j.Location,
            EmploymentType = j.EmploymentType,
            SalaryRange = j.SalaryRange,
            IsActive = j.IsActive,
            CreatedAt = j.CreatedAt,
            ClosingDate = j.ClosingDate,
            ApplicationsCount = j.Applications.Count
        }).ToListAsync();

        return Ok(list);
    }

    /// <summary>
    /// DIRECT ROUTE: ID'ye göre iş ilanı detayını getirir.
    /// </summary>
    [HttpGet("api/job-postings/{id:int}")]
    [ProducesResponseType(typeof(JobPostingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var j = await _db.JobPostings.Include(x => x.Company).Include(x => x.Applications).FirstOrDefaultAsync(x => x.Id == id);
        if (j == null)
            return NotFound(new { message = $"ID'si #{id} olan iş ilanı bulunamadı." });

        var dto = new JobPostingDto
        {
            Id = j.Id,
            CompanyId = j.CompanyId,
            CompanyName = j.Company.Name,
            Title = j.Title,
            Description = j.Description,
            Requirements = j.Requirements,
            Location = j.Location,
            EmploymentType = j.EmploymentType,
            SalaryRange = j.SalaryRange,
            IsActive = j.IsActive,
            CreatedAt = j.CreatedAt,
            ClosingDate = j.ClosingDate,
            ApplicationsCount = j.Applications.Count
        };

        return Ok(dto);
    }

    /// <summary>
    /// DIRECT ROUTE: Yeni iş ilanı oluşturur.
    /// </summary>
    [HttpPost("api/job-postings")]
    [ProducesResponseType(typeof(JobPostingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateJobPostingRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
            return BadRequest(new { message = "Başlık, açıklama ve geçerli bir şirket zorunludur." });

        var company = await _db.Companies.FindAsync(request.CompanyId);
        if (company == null)
            return BadRequest(new { message = $"ID'si #{request.CompanyId} olan şirket bulunamadı." });

        var posting = new JobPosting
        {
            CompanyId = request.CompanyId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Requirements = request.Requirements?.Trim(),
            Location = !string.IsNullOrWhiteSpace(request.Location) ? request.Location.Trim() : "İstanbul",
            EmploymentType = !string.IsNullOrWhiteSpace(request.EmploymentType) ? request.EmploymentType.Trim() : "Tam Zamanlı",
            SalaryRange = request.SalaryRange?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            ClosingDate = request.ClosingDate
        };

        _db.JobPostings.Add(posting);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = posting.Id }, new JobPostingDto
        {
            Id = posting.Id,
            CompanyId = posting.CompanyId,
            CompanyName = company.Name,
            Title = posting.Title,
            Description = posting.Description,
            Requirements = posting.Requirements,
            Location = posting.Location,
            EmploymentType = posting.EmploymentType,
            SalaryRange = posting.SalaryRange,
            IsActive = posting.IsActive,
            CreatedAt = posting.CreatedAt,
            ClosingDate = posting.ClosingDate,
            ApplicationsCount = 0
        });
    }

    /// <summary>
    /// DIRECT ROUTE: İş ilanını günceller.
    /// </summary>
    [HttpPut("api/job-postings/{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateJobPostingRequest request)
    {
        var posting = await _db.JobPostings.FindAsync(id);
        if (posting == null)
            return NotFound(new { message = $"ID'si #{id} olan iş ilanı bulunamadı." });

        posting.Title = request.Title.Trim();
        posting.Description = request.Description.Trim();
        posting.Requirements = request.Requirements?.Trim();
        posting.Location = request.Location.Trim();
        posting.EmploymentType = request.EmploymentType.Trim();
        posting.SalaryRange = request.SalaryRange?.Trim();
        posting.IsActive = request.IsActive;
        posting.ClosingDate = request.ClosingDate;

        await _db.SaveChangesAsync();
        return Ok(new { message = "İş ilanı başarıyla güncellendi." });
    }

    /// <summary>
    /// DIRECT ROUTE: İş ilanını siler.
    /// </summary>
    [HttpDelete("api/job-postings/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var posting = await _db.JobPostings.FindAsync(id);
        if (posting == null)
            return NotFound(new { message = $"ID'si #{id} olan iş ilanı bulunamadı." });

        _db.JobPostings.Remove(posting);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // NESTED ROUTES (İç İçe Rotalar)
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// NESTED ROUTE: Belirli bir şirkete ait iş ilanlarını getirir.
    /// Route: GET /api/companies/{companyId}/job-postings
    /// </summary>
    [HttpGet("api/companies/{companyId:int}/job-postings")]
    [ProducesResponseType(typeof(IEnumerable<JobPostingDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCompany(int companyId)
    {
        var list = await _db.JobPostings
            .Include(j => j.Company)
            .Include(j => j.Applications)
            .Where(j => j.CompanyId == companyId)
            .OrderByDescending(j => j.CreatedAt)
            .Select(j => new JobPostingDto
            {
                Id = j.Id,
                CompanyId = j.CompanyId,
                CompanyName = j.Company.Name,
                Title = j.Title,
                Description = j.Description,
                Requirements = j.Requirements,
                Location = j.Location,
                EmploymentType = j.EmploymentType,
                SalaryRange = j.SalaryRange,
                IsActive = j.IsActive,
                CreatedAt = j.CreatedAt,
                ClosingDate = j.ClosingDate,
                ApplicationsCount = j.Applications.Count
            }).ToListAsync();

        return Ok(list);
    }

    /// <summary>
    /// NESTED ROUTE: Belirli bir iş ilanına yapılmış tüm başvuruları getirir.
    /// Route: GET /api/job-postings/{jobPostingId}/applications
    /// </summary>
    [HttpGet("api/job-postings/{jobPostingId:int}/applications")]
    [ProducesResponseType(typeof(IEnumerable<JobApplicationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApplicationsForJob(int jobPostingId)
    {
        var exists = await _db.JobPostings.AnyAsync(j => j.Id == jobPostingId);
        if (!exists)
            return NotFound(new { message = $"ID'si #{jobPostingId} olan iş ilanı bulunamadı." });

        var apps = await _db.JobApplications
            .Include(a => a.JobPosting).ThenInclude(jp => jp.Company)
            .Include(a => a.User)
            .Where(a => a.JobPostingId == jobPostingId)
            .OrderByDescending(a => a.AppliedAt)
            .Select(a => new JobApplicationDto
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

        return Ok(apps);
    }

    /// <summary>
    /// NESTED ROUTE: Belirli bir iş ilanına yeni başvuru yapar.
    /// Route: POST /api/job-postings/{jobPostingId}/applications
    /// </summary>
    [HttpPost("api/job-postings/{jobPostingId:int}/applications")]
    [ProducesResponseType(typeof(JobApplicationDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> ApplyToJob(int jobPostingId, [FromBody] CreateJobApplicationRequest request)
    {
        var job = await _db.JobPostings.Include(j => j.Company).FirstOrDefaultAsync(j => j.Id == jobPostingId);
        if (job == null)
            return NotFound(new { message = $"ID'si #{jobPostingId} olan iş ilanı bulunamadı." });

        // Varsa User'ı kontrol et, yoksa ilk kullanıcıyı bağla
        var user = await _db.Users.FindAsync(request.UserId) ?? await _db.Users.FirstOrDefaultAsync();
        if (user == null)
        {
            user = new User
            {
                FirstName = request.ApplicantName ?? "Aday",
                LastName = "Kullanıcı",
                Email = request.ApplicantEmail ?? "aday@alumni.edu.tr",
                PasswordHash = "TempPassword123!",
                Role = Domain.Enums.UserRole.Alumni,
                IsActive = true
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        var application = new JobApplication
        {
            JobPostingId = jobPostingId,
            UserId = user.Id,
            ApplicantName = !string.IsNullOrWhiteSpace(request.ApplicantName) ? request.ApplicantName : $"{user.FirstName} {user.LastName}",
            ApplicantEmail = !string.IsNullOrWhiteSpace(request.ApplicantEmail) ? request.ApplicantEmail : user.Email,
            CoverLetter = request.CoverLetter,
            ResumeUrl = request.ResumeUrl,
            Status = "Beklemede",
            AppliedAt = DateTime.UtcNow
        };

        _db.JobApplications.Add(application);
        await _db.SaveChangesAsync();

        var dto = new JobApplicationDto
        {
            Id = application.Id,
            JobPostingId = application.JobPostingId,
            JobTitle = job.Title,
            CompanyName = job.Company.Name,
            UserId = user.Id,
            ApplicantName = application.ApplicantName!,
            ApplicantEmail = application.ApplicantEmail!,
            CoverLetter = application.CoverLetter,
            ResumeUrl = application.ResumeUrl,
            Status = application.Status,
            AppliedAt = application.AppliedAt
        };

        return Created($"api/applications/{application.Id}", dto);
    }
}
