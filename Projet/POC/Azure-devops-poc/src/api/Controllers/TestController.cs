using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        message = "Testing from Azure POC",
        timestamp = DateTime.UtcNow
    });

    [HttpGet("ping")]
    public string Ping() => "pong";
}
