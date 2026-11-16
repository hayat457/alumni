using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Alumni.API.Middlewares;

/// <summary>
/// 5xx Guardian (Global Exception Handling Middleware):
/// Sunucu tarafında oluşan tüm beklenmedik hataları (Unhandled Exceptions / 500 Internal Server Error) yakalar,
/// istemciye standart JSON hata formatı döner ve hatanın detaylarını proje kök dizinindeki 'logs/error.logs' dosyasına yazar.
/// </summary>
public class ExceptionGuardianMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionGuardianMiddleware> _logger;
    private readonly IWebHostEnvironment _env;
    private static readonly object _lock = new();

    public ExceptionGuardianMiddleware(RequestDelegate next, ILogger<ExceptionGuardianMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "5xx Guardian yakaladı: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var timestamp = DateTime.UtcNow;
        var method = context.Request.Method;
        var path = context.Request.Path + context.Request.QueryString;

        // 1. Hatayı logs/error.logs dosyasına yaz
        LogToFile(timestamp, method, path, exception);

        // 2. İstemciye 500 JSON yanıtı hazırla
        if (!context.Response.HasStarted)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var errorResponse = new
            {
                statusCode = StatusCodes.Status500InternalServerError,
                error = "Internal Server Error",
                message = "Sunucuda beklenmeyen bir hata meydana geldi.",
                detail = _env.IsDevelopment() ? exception.Message : "Lütfen daha sonra tekrar deneyiniz.",
                path = path,
                method = method,
                timestamp = timestamp
            };

            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }

    private void LogToFile(DateTime timestamp, string method, string path, Exception exception)
    {
        try
        {
            // Proje ana dizinini belirle (Alumni.API dizini veya solution ana dizini)
            var baseDirectory = AppContext.BaseDirectory;
            // Solution kökünü bulmaya çalış; aksi halde ContentRootPath kullan
            var projectRoot = _env.ContentRootPath;
            var logsDir = Path.Combine(projectRoot, "logs");

            if (!Directory.Exists(logsDir))
            {
                Directory.CreateDirectory(logsDir);
            }

            var filePath = Path.Combine(logsDir, "error.logs");

            var sb = new StringBuilder();
            sb.AppendLine("================================================================================");
            sb.AppendLine($"[ERROR LOG] {timestamp:yyyy-MM-dd HH:mm:ss.fff} UTC");
            sb.AppendLine($"HTTP {method} {path}");
            sb.AppendLine($"Exception Type: {exception.GetType().FullName}");
            sb.AppendLine($"Message: {exception.Message}");
            sb.AppendLine("Stack Trace:");
            sb.AppendLine(exception.StackTrace);
            if (exception.InnerException != null)
            {
                sb.AppendLine($"Inner Exception: {exception.InnerException.Message}");
                sb.AppendLine(exception.InnerException.StackTrace);
            }
            sb.AppendLine("================================================================================");
            sb.AppendLine();

            lock (_lock)
            {
                File.AppendAllText(filePath, sb.ToString(), Encoding.UTF8);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "logs/error.logs dosyasına yazılırken hata oluştu: {Message}", ex.Message);
        }
    }
}

public static class ExceptionGuardianMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionGuardian(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionGuardianMiddleware>();
    }
}
