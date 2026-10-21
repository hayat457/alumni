using Alumni.Business.Services;
using Alumni.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Alumni.API.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class HomeController : Controller
{
    private readonly AlumniDbContext _db;
    private readonly IInMemoryUserStore _userStore;
    private readonly IAnnouncementStore _announcementStore;

    public HomeController(AlumniDbContext db, IInMemoryUserStore userStore, IAnnouncementStore announcementStore)
    {
        _db = db;
        _userStore = userStore;
        _announcementStore = announcementStore;
    }

    [HttpGet("/")]
    public async Task<IActionResult> Index()
    {
        ViewBag.TotalUsers = _userStore.GetAll().Count();
        ViewBag.TotalJobs = await _db.JobPostings.CountAsync();
        ViewBag.TotalApplications = await _db.JobApplications.CountAsync();
        ViewBag.TotalAnnouncements = _announcementStore.GetAll().Count();

        ViewBag.LatestJobs = await _db.JobPostings.Include(j => j.Company).OrderByDescending(j => j.CreatedAt).Take(4).ToListAsync();
        ViewBag.LatestAnnouncements = _announcementStore.GetAll().Take(3).ToList();

        return View("~/Views/Home/Index.cshtml");
    }
}
