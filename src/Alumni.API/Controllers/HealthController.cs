using Alumni.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly AlumniDbContext _db;

    public HealthController(AlumniDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Sistemin ve veritabanı bağlantısının durumunu döner.
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        bool dbOk;
        try
        {
            dbOk = _db.Database.CanConnect();
        }
        catch
        {
            dbOk = false;
        }

        var result = new
        {
            status  = dbOk ? "healthy" : "degraded",
            service = "Alumni Platform API",
            version = "1.0.0",
            database = dbOk ? "connected" : "unreachable",
            timestamp = DateTime.UtcNow
        };

        return dbOk ? Ok(result) : StatusCode(503, result);
    }
}
