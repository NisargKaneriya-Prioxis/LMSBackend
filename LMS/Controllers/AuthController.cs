// using LM.Common;
// using LM.Model.RequestModel;
// using LM.Services.Repositories.Interface;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// [ApiController]
// [Route("[controller]")]
// public class AuthController : ControllerBase
// {
//     private readonly IAuthRepository _authRepository;
//     private readonly ILogger<AuthController> _logger;
//
//     public AuthController(IAuthRepository authRepository, ILogger<AuthController> logger)
//     {
//         _authRepository = authRepository;
//         _logger = logger;
//     }
//
//     [HttpGet("protected")]
//     [Authorize] 
//     public IActionResult Protected()
//     {
//         var userId = User.FindFirst("userId")?.Value; 
//         var role = User.FindFirst("role")?.Value;
//         return Ok(new { message = "Token is valid!", userId, role });
//     }
//     
//     [HttpPost("Login")]
//     public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
//     {
//         try
//         {
//             var response = await _authRepository.LoginUser(model);
//
//             return Ok(new
//             {
//                 response.UserSid,
//                 response.Name,
//                 response.Email,
//                 response.Role,
//                 message = "Login successful",
//                 token = response.Token 
//             });
//         }
//         catch (HttpStatusCodeException ex)
//         {
//             return StatusCode(ex.StatusCode, new { message = ex.Message });
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Unexpected error during login");
//             return StatusCode(500, new { message = "Internal server error" });
//         }
//     }
//     
//     [Authorize]
//     [HttpPost("Logout")]
//     public IActionResult Logout()
//     {
//         return Ok(new { message = "Logout endpoint available. Token removal depends on chosen strategy (stateless or stateful)." });
//     }
//
//     
// }
// --------------------------------------------
 // using System.IdentityModel.Tokens.Jwt;
 // using System.Security.Claims;
 using LM.Common;
 // using LM.Model.RequestModel;
 // using LM.Services.Repositories.Interface;
 // using LM.Services.Token;
 // using Microsoft.AspNetCore.Authorization;
 // using Microsoft.AspNetCore.Mvc;
 //
 // [ApiController]
 // [Route("[controller]")]
 // public class AuthController : ControllerBase
 // {
 //     private readonly IAuthRepository _authRepository;
 //     private readonly TokenService _tokenService;
 //     private readonly ILogger<AuthController> _logger;
 //
 //     public AuthController(IAuthRepository authRepository, TokenService tokenService, ILogger<AuthController> logger)
 //     {
 //         _authRepository = authRepository;
 //         _tokenService = tokenService;
 //         _logger = logger;
 //     }
 //
 //     // A simple protected endpoint to check token validity
 //     [HttpGet("protected")]
 //     [Authorize]
 //     public IActionResult Protected()
 //     {
 //         var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
 //         var role = User.FindFirst(ClaimTypes.Role)?.Value;
 //
 //         return Ok(new
 //         {
 //             message = "Token is valid!",
 //             userId,
 //             role
 //         });
 //     }
 //
 //     // Login endpoint
 //     [HttpPost("Login")]
 //     public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
 //     {
 //         try
 //         {
 //             // Authenticate user
 //             var user = await _authRepository.LoginUser(model);
 //
 //             if (user == null)
 //             {
 //                 return Unauthorized(new { message = "Invalid email or password." });
 //             }
 //
 //             // Generate a new JWT token
 //             var token = _tokenService.GenerateToken(user.UserSID.ToString(), user.Role, user.Email);
 //
 //
 //             return Ok(new
 //             {
 //                 userSid = user.UserSID,
 //                 user.Name,
 //                 user.Email,
 //                 user.Role,
 //                 message = "Login successful",
 //                 token
 //             });
 //         }
 //         catch (HttpStatusCodeException ex)
 //         {
 //             _logger.LogWarning("HTTP error during login: {Message}", ex.Message);
 //             return StatusCode(ex.StatusCode, new { message = ex.Message });
 //         }
 //         catch (Exception ex)
 //         {
 //             _logger.LogError(ex, "Unexpected error during login");
 //             return StatusCode(500, new { message = "Internal server error" });
 //         }
 //     }
 //
 //     // Logout endpoint (optional, depends on token strategy)
 //     [Authorize]
 //     [HttpPost("Logout")]
 //     public IActionResult Logout()
 //     {
 //         return Ok(new
 //         {
 //             message = "Logout endpoint available. Token removal depends on chosen strategy (stateless or stateful)."
 //         });
 //     }
 // }
 

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LM.Common;
using LM.Model.RequestModel;
using LM.Services.Repositories.Interface;
using LM.Services.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly TokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthRepository authRepository, TokenService tokenService, ILogger<AuthController> logger)
    {
        _authRepository = authRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    // A simple protected endpoint to check token validity
    [HttpGet("protected")]
    [Authorize]
    public IActionResult Protected()
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new
        {
            message = "Token is valid!",
            userId,
            role
        });
    }

    // Login endpoint
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
    {
        try
        {
            var response = await _authRepository.LoginUser(model);
            
            
            if (response == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }
            
            return Ok(new
            {
                userSid = response.UserSid,
                response.Name,
                response.Email,
                response.Role,
                message = "Login successful",
                token = response.Token
            });
        }
        catch (HttpStatusCodeException ex)
        {
            _logger.LogWarning("HTTP error during login: {Message}", ex.Message);
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
    
    [Authorize]
    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        return Ok(new
        {
            message = "Logout endpoint available. Token removal depends on chosen strategy (stateless or stateful)."
        });
    }
}
