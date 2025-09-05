using LM.Common;
using LM.Model.Models.MyLMSDB;
using LM.Model.RequestModel;
using LM.Model.ResponseModel;
using LM.Services.Repositories.Interface;
using LM.Services.Token;
using LM.Services.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace LM.Services.Repositories.Implementation;

public class AuthRepository : IAuthRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly TokenService _tokenService;
    private readonly ILogger<AuthRepository> _logger;

    public AuthRepository(IUnitOfWork unitOfWork, TokenService tokenService, ILogger<AuthRepository> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginResponseModel> LoginUser(LoginRequestModel model)
    {
        _logger.LogInformation("Attempting login for email: {Email}", model.Email);

        if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
        {
            _logger.LogWarning("Login failed: Email and Password are required");
            throw new HttpStatusCodeException(400, "Email and Password are required.");
        }

        var user = (await _unitOfWork.GetRepository<User>()
            .GetAllAsync(u => u.Email == model.Email && u.Status == (int)Enums.Active))
            .FirstOrDefault();

        if (user == null)
        {
            _logger.LogWarning("Login failed: User not found for email: {Email}", model.Email);
            throw new HttpStatusCodeException(401, "Invalid email or password.");
        }

        if (!PasswordHelper.VerifyPassword(model.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed: Invalid password for email: {Email}", model.Email);
            throw new HttpStatusCodeException(401, "Invalid email or password.");
        }

        var token = _tokenService.GenerateToken(
            user.UserSid,
            user.Role,
            user.Email
        );

        var response = new LoginResponseModel
        {
            UserSid = user.UserSid,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Token = token
        };

        _logger.LogInformation("Login successful for email: {Email}", model.Email);
        return response;
    }
}
