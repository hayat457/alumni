using Alumni.Business.DTOs.Announcement;
using Alumni.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers;

[Route("announcements")]
[ApiExplorerSettings(IgnoreApi = true)]
public class AnnouncementsViewController : Controller
{
    private readonly IAnnouncementStore _store;

    public AnnouncementsViewController(IAnnouncementStore store)
    {
        _store = store;
    }

    /// <summary>GET /announcements -> Duyuru listesi görünümü.</summary>
    [HttpGet("")]
    public IActionResult Index()
    {
        var list = _store.GetAll();
        return View("~/Views/Announcements/Index.cshtml", list);
    }

    /// <summary>GET /announcements/create -> Yeni duyuru oluşturma sayfası.</summary>
    [HttpGet("create")]
    public IActionResult Create()
    {
        return View("~/Views/Announcements/Create.cshtml", new CreateAnnouncementRequest());
    }

    /// <summary>POST /announcements/create veya POST /announcements -> Duyuruyu kaydeder.</summary>
    [HttpPost("create")]
    [HttpPost("")]
    public IActionResult Create([FromForm] CreateAnnouncementRequest request)
    {
        var (newAnnouncement, error) = _store.Create(request);
        if (error != null)
        {
            TempData["ErrorMessage"] = error;
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = $"'{newAnnouncement!.Title}' başlıklı duyuru başarıyla yayımlandı!";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>GET /announcements/details/{id} -> Duyuru detay görünümü.</summary>
    [HttpGet("details/{id:int}")]
    [HttpGet("{id:int}")]
    public IActionResult Details(int id)
    {
        var announcement = _store.GetById(id);
        if (announcement == null)
        {
            TempData["ErrorMessage"] = $"ID'si #{id} olan duyuru bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        return View("~/Views/Announcements/Details.cshtml", announcement);
    }

    /// <summary>GET /announcements/edit/{id} -> Duyuru düzenleme sayfası.</summary>
    [HttpGet("edit/{id:int}")]
    public IActionResult Edit(int id)
    {
        var announcement = _store.GetById(id);
        if (announcement == null)
        {
            TempData["ErrorMessage"] = $"ID'si #{id} olan duyuru bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        return View("~/Views/Announcements/Edit.cshtml", announcement);
    }

    /// <summary>POST /announcements/edit/{id} -> Duyuru güncellemesini kaydeder.</summary>
    [HttpPost("edit/{id:int}")]
    public IActionResult Edit(int id, [FromForm] UpdateAnnouncementRequest request)
    {
        var (updated, error) = _store.Update(id, request);
        if (error != null)
        {
            TempData["ErrorMessage"] = error;
            return RedirectToAction(nameof(Edit), new { id });
        }

        TempData["SuccessMessage"] = $"#{id} numaralı duyuru başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>POST /announcements/delete/{id} -> Duyuruyu siler.</summary>
    [HttpPost("delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        var announcement = _store.GetById(id);
        var title = announcement != null ? announcement.Title : $"#{id}";

        if (_store.Delete(id))
        {
            TempData["SuccessMessage"] = $"'{title}' duyurusu silindi.";
        }
        else
        {
            TempData["ErrorMessage"] = $"ID'si #{id} olan duyuru bulunamadı.";
        }

        return RedirectToAction(nameof(Index));
    }
}
