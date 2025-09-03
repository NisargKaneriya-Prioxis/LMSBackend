using EvaluationAPI.Controllers;
using LM.Common;
using LM.Model.RequestModel;
using LM.Model.ResponseModel;
using LM.Services.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace LMS.Controllers;

public class UserController : BaseController
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserRepository userRepository, ILogger<UserController> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }
    
    
    //get all controller 
    [HttpGet("getalluser")]
    public async Task<ActionResult<IEnumerable<LMSUserResponseModel>>> Getalluser([FromQuery] SearchRequestModel model)
    {
        _logger.LogInformation("Fetching all Users with search params: {@SearchRequest}", model);

        var parameters = FillParamesFromModel(model);
        var list = await _userRepository.List(parameters);
        

        if (list != null)
        {
            var result = JsonConvert.DeserializeObject<List<LMSUserResponseModel>>(list.Result?.ToString() ?? "[]") ?? [];
            list.Result = result;
            _logger.LogInformation("Retrieved {Count} users", result.Count);
            return Ok(BindSearchResult(list, model, "user list"));
        }

        _logger.LogWarning("No user found matching search parameters");
        return NoContent();
    }
    
    //get by sid controller 
    [HttpGet("dynamicuser/{usersid}")]
    public async Task<ActionResult<LMSUserResponseModel>> GetByuserSID([FromRoute] string usersid)
    {
        _logger.LogInformation("Fetching book with SID: {Sid}", usersid);

        var user = await _userRepository.GetuserBySID(usersid);

        if (user == null)
        {
            _logger.LogWarning("Book not found with SID: {Sid}", usersid);
            return NotFound(new { message = $"Book with SID '{usersid}' not found" });
        }

        _logger.LogInformation("Successfully retrieved book: (SID: {Sid})", usersid);
        return Ok(user);
    }
    
    
    //Insert user 
    [HttpPost("InsertUser")]
    public async Task<ActionResult<List<LMSUserResponseModel>>> InsertBook([FromBody] List<UserSignupRequestModel> user)
    {
        List<LMSUserResponseModel> createduser = await _userRepository.InsertUser(user);
        if (createduser == null)
        {
            _logger.LogInformation("Failed to create user: {@userdata}", user);
            return BadRequest();
        }

        _logger.LogInformation("user created successfully");
        return Ok(createduser);
    }
    
    //login controller 
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _userRepository.LoginUser(model);
            return Ok(result);
        }
        catch (HttpStatusCodeException ex)
        {
            _logger.LogWarning("Login failed: {Message}", ex.Message);
            return StatusCode(ex.StatusCode, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred during login.");
            return StatusCode(500, new { error = "An unexpected error occurred." });
        }
    }

    //delete controller
    [HttpDelete("deleteuser/{userSid}")]
    public async Task<ActionResult> DeleteUser([FromRoute] string userSid)
    {
        _logger.LogInformation("Request to delete book with SID: {Sid}", userSid);

        var result = await _userRepository.Deleteuser(userSid);

        if (!result)
        {
            _logger.LogWarning("User not found for deletion. SID: {Sid}", userSid);
            return NotFound($"User with SID '{userSid}' not found.");
        }

        _logger.LogInformation("Successfully deleted book with SID: {Sid}", userSid);
        return Ok("User deleted.");
    }
}