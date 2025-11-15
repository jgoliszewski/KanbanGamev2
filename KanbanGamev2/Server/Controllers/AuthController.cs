using Microsoft.AspNetCore.Mvc;

namespace KanbanGamev2.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly string _gameMasterPassword;

    public AuthController(IConfiguration configuration)
    {
        _gameMasterPassword = configuration["GameMaster:Password"] ?? "professor123";
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { IsAuthenticated = false, Message = "Password is required" });
        }

        // Simple password comparison (for classroom use)
        if (request.Password == _gameMasterPassword)
        {
            return Ok(new { IsAuthenticated = true });
        }

        return Unauthorized(new { IsAuthenticated = false, Message = "Invalid password" });
    }

    public class LoginRequest
    {
        public string Password { get; set; } = string.Empty;
    }
}

