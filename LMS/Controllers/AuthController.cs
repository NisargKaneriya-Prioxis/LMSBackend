using LM.Common;
using LM.Model.RequestModel;
using LM.Services.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthRepository authRepository, ILogger<AuthController> logger)
    {
        _authRepository = authRepository;
        _logger = logger;
    }

    [HttpGet("protected")]
    [Authorize] 
    public IActionResult Protected()
    {
        var userId = User.FindFirst("userId")?.Value; 
        var role = User.FindFirst("role")?.Value;
        return Ok(new { message = "Token is valid!", userId, role });
    }
    
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
    {
        try
        {
            var response = await _authRepository.LoginUser(model);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("jwt", response.Token, cookieOptions);

            return Ok(new
            {
                response.UserSid,
                response.Name,
                response.Email,
                response.Role,
                message = "Login successful",
                token = response.Token
            });
        }
        catch (HttpStatusCodeException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok(new { message = "Logged out successfully" });
    }
    
}