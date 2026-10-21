using Alumni.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Alumni.API.Controllers;

[Route("applications")]
[ApiExplorerSettings(IgnoreApi = true)]
public class ApplicationsViewController : Controller
{
    private readonly AlumniDbContext _db;

    public ApplicationsViewController(AlumniDbContext db)
    {
        _db = db;
    }

    /// <summary>Başvurular listesi görünümü.</summary>
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var applications = await _db.JobApplications
            .Include(a => a.JobPosting).ThenInclude(jp => jp.Company)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();

        return View("~/Views/Applications/Index.cshtml", applications);
    }
}
