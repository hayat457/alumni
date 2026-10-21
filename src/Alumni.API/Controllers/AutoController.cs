using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers;

/// <summary>
/// Temel rota alıştırmaları (Whiteboard Routes: /, /hello, /hello/{name}, /sum/{number1}/{number2}).
/// </summary>
[ApiController]
public class AutoController : ControllerBase
{
    /// <summary>
    /// ① GET / ve GET /auto -> "ok"
    /// </summary>
    [HttpGet("/")]
    [HttpGet("auto")]
    [Produces("text/plain")]
    public IActionResult GetRoot()
    {
        return Ok("ok");
    }

    /// <summary>
    /// ② GET /hello ve GET /auto/hello -> "Hello, World!"
    /// </summary>
    [HttpGet("hello")]
    [HttpGet("auto/hello")]
    [Produces("text/plain")]
    public IActionResult GetHello()
    {
        return Ok("Hello, World!");
    }

    /// <summary>
    /// ③ GET /hello/{name} ve GET /auto/hello/{name} -> "Hello, {Name}!" (Örn: /hello/emre -> "Hello, Emre!")
    /// </summary>
    [HttpGet("hello/{name}")]
    [HttpGet("auto/hello/{name}")]
    [Produces("text/plain")]
    public IActionResult GetHelloByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Ok("Hello, World!");

        // İlk harfi büyük yapar (örn: emre -> Emre)
        var formattedName = char.ToUpper(name[0]) + (name.Length > 1 ? name.Substring(1) : string.Empty);
        return Ok($"Hello, {formattedName}!");
    }

    /// <summary>
    /// ④ GET /sum/{number1}/{number2} ve GET /auto/sum/{number1}/{number2} -> İki sayının toplamını döner.
    /// </summary>
    [HttpGet("sum/{number1}/{number2}")]
    [HttpGet("auto/sum/{number1}/{number2}")]
    [Produces("text/plain")]
    public IActionResult GetSum(string number1, string number2)
    {
        if (double.TryParse(number1, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var n1) &&
            double.TryParse(number2, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var n2))
        {
            var sum = n1 + n2;
            return Ok(sum.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        return BadRequest("Geçersiz sayı parametresi.");
    }
}
