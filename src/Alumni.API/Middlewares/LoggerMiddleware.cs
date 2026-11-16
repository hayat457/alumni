using System.Diagnostics;

namespace Alumni.API.Middlewares;

/// <summary>
/// Gelen her HTTP isteğini konsola şu sütun formatında loglar:
/// time | method | route | res type (status code) | duration
/// Örnek çıktı satırı: 10:20:30 POST /api/users 201 18ms
/// </summary>
public class LoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggerMiddleware> _logger;

    public LoggerMiddleware(RequestDelegate next, ILogger<LoggerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // İstek zamanı (HH:mm:ss formatında)
        var time = DateTime.Now.ToString("HH:mm:ss");
        var method = context.Request.Method;
        var path = context.Request.Path + context.Request.QueryString;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            var duration = $"{stopwatch.ElapsedMilliseconds}ms";

            // Talep edilen format: time | method | route | res type (status code) | duration
            // Örnek: 10:20:30 POST /api/users 201 18ms
            var logLine = $"{time} {method} {path} {statusCode} {duration}";

            // Hem doğrudan Terminal / Konsol çıktısına hem de ILogger'a yazdırıyoruz
            Console.WriteLine(logLine);
            _logger.LogInformation("{Time} {Method} {Path} {StatusCode} {Duration}", time, method, path, statusCode, duration);
        }
    }
}

public static class LoggerMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogger(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoggerMiddleware>();
    }
}
