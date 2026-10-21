using Alumni.Business.DTOs.User;
using Alumni.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers;

[Route("users")]
[ApiExplorerSettings(IgnoreApi = true)]
public class UsersViewController : Controller
{
    private readonly IInMemoryUserStore _userStore;

    public UsersViewController(IInMemoryUserStore userStore)
    {
        _userStore = userStore;
    }

    /// <summary>
    /// READ ALL: GET /users -> Tüm kullanıcıları listeler.
    /// </summary>
    [HttpGet("")]
    public IActionResult Index()
    {
        var users = _userStore.GetAll();
        return View("~/Views/Users/Index.cshtml", users);
    }

    /// <summary>
    /// CREATE VIEW: GET /users/create -> Kullanıcı oluşturma form sayfasını açar.
    /// </summary>
    [HttpGet("create")]
    public IActionResult Create()
    {
        return View("~/Views/Users/Create.cshtml", new CreateUserRequest());
    }

    /// <summary>
    /// CREATE ACTION: POST /users/create veya POST /users -> Formu kaydeder.
    /// </summary>
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

    /// <summary>
    /// READ ONE (DETAILS): GET /users/details/{id} veya GET /users/{id} -> Detay sayfasını açar.
    /// </summary>
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

    /// <summary>
    /// UPDATE VIEW: GET /users/edit/{id} -> Düzenleme form sayfasını açar.
    /// </summary>
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

    /// <summary>
    /// UPDATE ACTION: POST /users/edit/{id} -> Düzenleme formunu kaydeder.
    /// </summary>
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

    /// <summary>
    /// DELETE ACTION: POST /users/delete/{id} -> Kullanıcıyı siler.
    /// </summary>
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
