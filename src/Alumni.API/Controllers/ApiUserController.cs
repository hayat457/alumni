using Alumni.Business.DTOs.User;
using Alumni.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers;

/// <summary>
/// ApiUserController: Kullanıcı yönetimi REST API endpoint'leri (Veritabanı olmadan / without database).
/// Route: /api/users
/// </summary>
[ApiController]
[Route("api/users")]
public class ApiUserController : ControllerBase
{
    private readonly IInMemoryUserStore _userStore;

    public ApiUserController(IInMemoryUserStore userStore)
    {
        _userStore = userStore;
    }

    /// <summary>READ (ALL): Tüm kullanıcıları listeler.</summary>
    /// <response code="200">Kullanıcı listesi başarıyla döndürüldü.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        return Ok(_userStore.GetAll());
    }

    /// <summary>READ (ONE): ID'ye göre kullanıcı detayını getirir.</summary>
    /// <param name="id">Sorgulanacak kullanıcının ID'si.</param>
    /// <response code="200">Kullanıcı bulundu ve başarıyla döndürüldü.</response>
    /// <response code="404">Belirtilen ID'ye sahip kullanıcı bulunamadı.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var user = _userStore.GetById(id);
        if (user != null)
            return Ok(user);

        return NotFound(new { message = $"ID'si #{id} olan kullanıcı bulunamadı." });
    }

    /// <summary>CREATE: Yeni kullanıcı oluşturur (without database connection).</summary>
    /// <param name="request">Oluşturulacak kullanıcının bilgileri.</param>
    /// <response code="201">Kullanıcı başarıyla oluşturuldu.</response>
    /// <response code="400">Eksik veya geçersiz alanlar.</response>
    /// <response code="409">E-posta adresi zaten mevcut.</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Create([FromBody] CreateUserRequest request)
    {
        var (newUser, error) = _userStore.Create(request);

        if (error != null)
        {
            if (error.Contains("zaten mevcut"))
                return Conflict(new { message = error });

            return BadRequest(new { message = error });
        }

        return CreatedAtAction(nameof(GetById), new { id = newUser!.Id }, newUser);
    }

    /// <summary>UPDATE (PUT): Kullanıcıyı tamamen günceller.</summary>
    /// <param name="id">Güncellenecek kullanıcı ID'si.</param>
    /// <param name="request">Yeni kullanıcı bilgileri.</param>
    /// <response code="200">Kullanıcı başarıyla güncellendi.</response>
    /// <response code="400">Geçersiz istek.</response>
    /// <response code="404">Kullanıcı bulunamadı.</response>
    /// <response code="409">E-posta adresi başka bir kullanıcı tarafından kullanılıyor.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Update(int id, [FromBody] UpdateUserRequest request)
    {
        var (updatedUser, error) = _userStore.Update(id, request);

        if (error != null)
        {
            if (error.Contains("bulunamadı"))
                return NotFound(new { message = error });
            if (error.Contains("başka bir kullanıcı"))
                return Conflict(new { message = error });
            return BadRequest(new { message = error });
        }

        return Ok(updatedUser);
    }

    /// <summary>UPDATE (PATCH): Kullanıcıyı kısmi günceller.</summary>
    /// <param name="id">Güncellenecek kullanıcı ID'si.</param>
    /// <param name="request">Kısmi olarak değiştirilecek alanlar.</param>
    /// <response code="200">Kullanıcı başarıyla güncellendi.</response>
    /// <response code="400">Geçersiz istek.</response>
    /// <response code="404">Kullanıcı bulunamadı.</response>
    /// <response code="409">E-posta adresi başka bir kullanıcı tarafından kullanılıyor.</response>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Patch(int id, [FromBody] PatchUserRequest request)
    {
        var (patchedUser, error) = _userStore.Patch(id, request);

        if (error != null)
        {
            if (error.Contains("bulunamadı"))
                return NotFound(new { message = error });
            if (error.Contains("başka bir kullanıcı"))
                return Conflict(new { message = error });
            return BadRequest(new { message = error });
        }

        return Ok(patchedUser);
    }

    /// <summary>DELETE: Kullanıcıyı siler.</summary>
    /// <param name="id">Silinecek kullanıcı ID'si.</param>
    /// <response code="204">Kullanıcı silindi.</response>
    /// <response code="404">Kullanıcı bulunamadı.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        if (_userStore.Delete(id))
            return NoContent();

        return NotFound(new { message = $"ID'si #{id} olan kullanıcı bulunamadı." });
    }
}
