using System.Net.Mime;
using System.Text.Json;

namespace Alumni.API.Middlewares;

/// <summary>
/// 404 Guardian Middleware:
/// Bulunamayan endpoint/kaynak isteklerini (404) yakalar ve:
/// - API istekleri (veya Accept: application/json) için açıklayıcı bir sebep (reason) içeren anlamlı JSON hata yanıtı döndürür.
/// - Tarayıcıdan gelen genel sayfa istekleri (text/html) için ise generic 404 sayfasına yönlendirir veya 404 HTML içeriği sunar.
/// </summary>
public class NotFoundGuardianMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<NotFoundGuardianMiddleware> _logger;

    public NotFoundGuardianMiddleware(RequestDelegate next, ILogger<NotFoundGuardianMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        // İstek henüz yanıt gövdesi yazılmamış ve 404 durum koduna sahipse müdahale et
        if (context.Response.StatusCode == StatusCodes.Status404NotFound && !context.Response.HasStarted)
        {
            var path = context.Request.Path.Value ?? "/";
            var method = context.Request.Method;
            var acceptHeader = context.Request.Headers.Accept.ToString();

            _logger.LogWarning("404 Guardian yakaladı: {Method} {Path}", method, path);

            // Eğer API rotası ise (/api/...) veya JSON talep ediliyorsa JSON formatında detaylı ve anlamlı hata dön
            var isApiRequest = path.StartsWith("/api", StringComparison.OrdinalIgnoreCase) ||
                               acceptHeader.Contains("application/json", StringComparison.OrdinalIgnoreCase);

            if (isApiRequest)
            {
                context.Response.ContentType = MediaTypeNames.Application.Json;

                var errorResponse = new
                {
                    statusCode = 404,
                    error = "Not Found",
                    reason = $"Belirtilen rota veya kaynak sunucuda bulunamadı: '{method} {path}'",
                    path = path,
                    method = method,
                    timestamp = DateTime.UtcNow,
                    suggestion = "Lütfen rota adını, parametreleri veya Swagger dokümantasyonunu (/swagger) kontrol ediniz."
                };

                var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(json);
            }
            else
            {
                // Tarayıcı doğrudan /404 sayfasına gitmeye çalışmıyorsa /404 sayfasına yönlendir
                if (!path.Equals("/404", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.Redirect("/404");
                }
            }
        }
    }
}

public static class NotFoundGuardianMiddlewareExtensions
{
    public static IApplicationBuilder UseNotFoundGuardian(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<NotFoundGuardianMiddleware>();
    }
}
