using Alumni.Business.DTOs.User;
using Alumni.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers;

/// <summary>
/// UserController: Kullanıcı yönetimi MVC View katmanı controller'ı (without database).
/// Route: /users
/// </summary>
[Route("users")]
[ApiExplorerSettings(IgnoreApi = true)]
public class UserController : Controller
{
    private readonly IInMemoryUserStore _userStore;

    public UserController(IInMemoryUserStore userStore)
    {
        _userStore = userStore;
    }

    /// <summary>READ ALL: GET /users -> Kullanıcıları listeleyen View.</summary>
    [HttpGet("")]
    public IActionResult Index()
    {
        var users = _userStore.GetAll();
        return View("~/Views/Users/Index.cshtml", users);
    }

    /// <summary>CREATE VIEW: GET /users/create -> Kullanıcı ekleme form sayfası.</summary>
    [HttpGet("create")]
    public IActionResult Create()
    {
        return View("~/Views/Users/Create.cshtml", new CreateUserRequest());
    }

    /// <summary>CREATE ACTION: POST /users/create veya POST /users -> Kullanıcı oluşturur.</summary>
    [HttpPost("create")]
    [HttpPost("")]
    public IActionResult Create([FromForm] CreateUserRequest request)
    {
        var (newUser, error) = _userStore.Create(request);
        if (error != null)
        {
            TempData["ErrorMessage"] = error;
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = $"'{newUser!.FirstName} {newUser.LastName}' başarıyla oluşturuldu! (ID: #{newUser.Id})";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>READ ONE (DETAILS): GET /users/details/{id} -> Kullanıcı detay görünümü.</summary>
    [HttpGet("details/{id:int}")]
    [HttpGet("{id:int}")]
    public IActionResult Details(int id)
    {
        var user = _userStore.GetById(id);
        if (user == null)
        {
            TempData["ErrorMessage"] = $"ID'si #{id} olan kullanıcı bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        return View("~/Views/Users/Details.cshtml", user);
    }

    /// <summary>UPDATE VIEW: GET /users/edit/{id} -> Kullanıcı düzenleme sayfası.</summary>
    [HttpGet("edit/{id:int}")]
    public IActionResult Edit(int id)
    {
        var user = _userStore.GetById(id);
        if (user == null)
        {
            TempData["ErrorMessage"] = $"ID'si #{id} olan kullanıcı bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        return View("~/Views/Users/Edit.cshtml", user);
    }

    /// <summary>UPDATE ACTION: POST /users/edit/{id} -> Güncellemeyi kaydeder.</summary>
    [HttpPost("edit/{id:int}")]
    public IActionResult Edit(int id, [FromForm] UpdateUserRequest request)
    {
        var (updatedUser, error) = _userStore.Update(id, request);
        if (error != null)
        {
            TempData["ErrorMessage"] = error;
            return RedirectToAction(nameof(Edit), new { id });
        }

        TempData["SuccessMessage"] = $"#{id} numaralı '{updatedUser!.FirstName} {updatedUser.LastName}' başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>DELETE ACTION: POST /users/delete/{id} -> Kullanıcıyı siler.</summary>
    [HttpPost("delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        var user = _userStore.GetById(id);
        var name = user != null ? $"{user.FirstName} {user.LastName}" : $"#{id}";

        if (_userStore.Delete(id))
        {
            TempData["SuccessMessage"] = $"'{name}' kullanıcısı başarıyla silindi.";
        }
        else
        {
            TempData["ErrorMessage"] = $"ID'si #{id} olan kullanıcı bulunamadı.";
        }

        return RedirectToAction(nameof(Index));
    }
}
