using Alumni.Business.DTOs.JobApplication;
using Alumni.Business.DTOs.JobPosting;
using Alumni.DataAccess.Context;
using Alumni.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Alumni.API.Controllers;

[Route("job-postings")]
[ApiExplorerSettings(IgnoreApi = true)]
public class JobPostingsViewController : Controller
{
    private readonly AlumniDbContext _db;

    public JobPostingsViewController(AlumniDbContext db)
    {
        _db = db;
    }

    /// <summary>İş ilanları listesi UI görünümü.</summary>
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var postings = await _db.JobPostings.Include(j => j.Company).Include(j => j.Applications).OrderByDescending(j => j.CreatedAt).ToListAsync();
        ViewBag.Companies = await _db.Companies.ToListAsync();
        return View("~/Views/JobPostings/Index.cshtml", postings);
    }

    /// <summary>Yeni ilan oluşturma sayfası.</summary>
    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Companies = await _db.Companies.ToListAsync();
        return View("~/Views/JobPostings/Create.cshtml", new CreateJobPostingRequest());
    }

    /// <summary>Yeni ilanı kaydeder.</summary>
    [HttpPost("create")]
    [HttpPost("")]
    public async Task<IActionResult> Create([FromForm] CreateJobPostingRequest request)
    {
        var company = await _db.Companies.FindAsync(request.CompanyId);
        if (company == null)
        {
            TempData["ErrorMessage"] = "Lütfen geçerli bir şirket seçiniz.";
            return RedirectToAction(nameof(Create));
        }

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

        TempData["SuccessMessage"] = $"'{posting.Title}' ilanı başarıyla yayımlandı!";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>İlan detay ve başvuru sayfası.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var posting = await _db.JobPostings.Include(j => j.Company).Include(j => j.Applications).FirstOrDefaultAsync(j => j.Id == id);
        if (posting == null)
        {
            TempData["ErrorMessage"] = "İlan bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        return View("~/Views/JobPostings/Details.cshtml", posting);
    }

    /// <summary>İlana başvuru yapma formu.</summary>
    [HttpGet("{id:int}/apply")]
    public async Task<IActionResult> Apply(int id)
    {
        var posting = await _db.JobPostings.Include(j => j.Company).FirstOrDefaultAsync(j => j.Id == id);
        if (posting == null)
        {
            TempData["ErrorMessage"] = "İlan bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        return View("~/Views/JobPostings/Apply.cshtml", posting);
    }

    /// <summary>Başvuru işlemini kaydeder.</summary>
    [HttpPost("{id:int}/apply")]
    public async Task<IActionResult> SubmitApplication(int id, [FromForm] string applicantName, [FromForm] string applicantEmail, [FromForm] string? coverLetter, [FromForm] string? resumeUrl)
    {
        var posting = await _db.JobPostings.FindAsync(id);
        if (posting == null) return NotFound();

        var user = await _db.Users.FirstOrDefaultAsync();
        if (user == null)
        {
            user = new User
            {
                FirstName = applicantName,
                LastName = "Aday",
                Email = applicantEmail,
                PasswordHash = "Secret123!",
                Role = Domain.Enums.UserRole.Alumni
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        var app = new JobApplication
        {
            JobPostingId = id,
            UserId = user.Id,
            ApplicantName = applicantName,
            ApplicantEmail = applicantEmail,
            CoverLetter = coverLetter,
            ResumeUrl = resumeUrl,
            Status = "Beklemede",
            AppliedAt = DateTime.UtcNow
        };

        _db.JobApplications.Add(app);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"'{posting.Title}' pozisyonuna başvurunuz başarıyla alındı!";
        return Redirect("/applications");
    }
}
