using Alumni.Business.DTOs.User;
using Alumni.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers;

/// <summary>
/// Kullanıcı yönetimi API endpoint'leri (In-Memory, veritabanı olmadan).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IInMemoryUserStore _userStore;

    public UsersController(IInMemoryUserStore userStore)
    {
        _userStore = userStore;
    }

    /// <summary>
    /// Yeni bir kullanıcı oluşturur (In-memory, veritabanı kullanılmaz).
    /// </summary>
    /// <param name="request">Oluşturulacak kullanıcının bilgileri.</param>
    /// <response code="201">Kullanıcı başarıyla oluşturuldu.</response>
    /// <response code="400">Geçersiz veya eksik alanlar gönderildi.</response>
    /// <response code="409">Bu e-posta adresiyle bir kullanıcı zaten kayıtlı.</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        var (newUser, error) = _userStore.Create(request);

        if (error != null)
        {
            if (error.Contains("zaten mevcut"))
                return Conflict(new { message = error });

            return BadRequest(new { message = error });
        }

        return CreatedAtAction(nameof(GetUserById), new { id = newUser!.Id }, newUser);
    }

    /// <summary>
    /// Tüm in-memory kullanıcıları listeler.
    /// </summary>
    /// <response code="200">Kullanıcı listesi başarıyla döndürüldü.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public IActionResult GetAllUsers()
    {
        return Ok(_userStore.GetAll());
    }

    /// <summary>
    /// ID'ye göre tek bir kullanıcının bilgilerini getirir.
    /// </summary>
    /// <param name="id">Sorgulanacak kullanıcının ID'si.</param>
    /// <response code="200">Kullanıcı bulundu ve başarıyla döndürüldü.</response>
    /// <response code="404">Belirtilen ID'ye sahip kullanıcı bulunamadı.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUserById(int id)
    {
        var user = _userStore.GetById(id);
        if (user != null)
            return Ok(user);

        return NotFound(new { message = $"ID'si {id} olan kullanıcı bulunamadı." });
    }

    /// <summary>
    /// ID ile belirtilen kullanıcıyı tamamen günceller (PUT).
    /// </summary>
    /// <param name="id">Güncellenecek kullanıcının ID'si.</param>
    /// <param name="request">Kullanıcının yeni bilgileri.</param>
    /// <response code="200">Kullanıcı başarıyla güncellendi.</response>
    /// <response code="400">Eksik veya geçersiz alanlar.</response>
    /// <response code="404">Kullanıcı bulunamadı.</response>
    /// <response code="409">E-posta adresi başka bir kullanıcı tarafından kullanılıyor.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult UpdateUser(int id, [FromBody] UpdateUserRequest request)
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

    /// <summary>
    /// ID ile belirtilen kullanıcının belirli alanlarını kısmi olarak günceller (PATCH).
    /// </summary>
    /// <param name="id">Güncellenecek kullanıcının ID'si.</param>
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
    public IActionResult PatchUser(int id, [FromBody] PatchUserRequest request)
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

    /// <summary>
    /// ID ile belirtilen kullanıcıyı siler (DELETE).
    /// </summary>
    /// <param name="id">Silinecek kullanıcının ID'si.</param>
    /// <response code="204">Kullanıcı başarıyla silindi (İçerik yok).</response>
    /// <response code="404">Kullanıcı bulunamadı.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteUser(int id)
    {
        if (_userStore.Delete(id))
            return NoContent();

        return NotFound(new { message = $"ID'si {id} olan kullanıcı bulunamadı." });
    }
}
