using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet("protected")]
    [Authorize] 
    public IActionResult Protected()
    {
        var userId = User.FindFirst("userId")?.Value; // Get user info from token
        var role = User.FindFirst("role")?.Value;
        return Ok(new { message = "Token is valid!", userId, role });
    }
}