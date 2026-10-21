using Alumni.Business.DTOs.Announcement;
using Alumni.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers;

/// <summary>
/// Duyuru yönetimi API endpoint'leri (In-Memory, veritabanı olmadan).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AnnouncementsController : ControllerBase
{
    private readonly IAnnouncementStore _store;

    public AnnouncementsController(IAnnouncementStore store)
    {
        _store = store;
    }

    /// <summary>Tüm duyuruları listeler.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AnnouncementDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        return Ok(_store.GetAll());
    }

    /// <summary>ID'ye göre duyuru detayını getirir.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AnnouncementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var announcement = _store.GetById(id);
        if (announcement == null)
            return NotFound(new { message = $"ID'si #{id} olan duyuru bulunamadı." });

        return Ok(announcement);
    }

    /// <summary>Yeni bir duyuru oluşturur.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AnnouncementDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] CreateAnnouncementRequest request)
    {
        var (newAnnouncement, error) = _store.Create(request);
        if (error != null)
            return BadRequest(new { message = error });

        return CreatedAtAction(nameof(GetById), new { id = newAnnouncement!.Id }, newAnnouncement);
    }

    /// <summary>Duyuruyu tamamen günceller (PUT).</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(AnnouncementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] UpdateAnnouncementRequest request)
    {
        var (updated, error) = _store.Update(id, request);
        if (error != null)
        {
            if (error.Contains("bulunamadı"))
                return NotFound(new { message = error });
            return BadRequest(new { message = error });
        }

        return Ok(updated);
    }

    /// <summary>Duyuruyu kısmi günceller (PATCH).</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(AnnouncementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Patch(int id, [FromBody] PatchAnnouncementRequest request)
    {
        var (patched, error) = _store.Patch(id, request);
        if (error != null)
            return NotFound(new { message = error });

        return Ok(patched);
    }

    /// <summary>Duyuruyu siler (DELETE).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        if (_store.Delete(id))
            return NoContent();

        return NotFound(new { message = $"ID'si #{id} olan duyuru bulunamadı." });
    }
}
