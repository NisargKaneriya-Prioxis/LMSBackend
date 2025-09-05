using EA.Services.RepositoryFactory;
using LM.Common;
using LM.Model.CommonModel;
using LM.Model.Models.MyLMSDB;
using LM.Model.RequestModel;
using LM.Model.ResponseModel;
using LM.Model.SpDbContext;
using LM.Services.Repositories.Interface;
using LM.Services.Token;
using LM.Services.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace LM.Services.Repositories.Implementation;

public class UserRepository: IUserRepository
{   
    private readonly LMSDbContext _context;
    private readonly ILogger<UserRepository> _logger;
    private readonly LMSSpContext _spContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TokenService _tokenService;

    
    
    public UserRepository(
        LMSDbContext context,
        IUnitOfWork unitOfWork,
        ILogger<UserRepository> logger,
        LMSSpContext spContext,
        TokenService tokenService
        )
    {
        _context = context;
        _logger = logger;
        _spContext = spContext;
        _unitOfWork = unitOfWork;
        _tokenService=tokenService;
    }

    
    //get all user
    public async Task<Page> List(Dictionary<string, object> parameters)
    {
        _logger.LogInformation("Fetching list of user with parameters: {@Params}", parameters);

        try
        {
            var xmlParam = CommonHelper.DictionaryToXml(parameters, "Search");
            string sqlQuery = "SP_GetAllUser {0}";
            object[] param = { xmlParam };

            var res = await _spContext.ExecutreStoreProcedureResultList(sqlQuery, param);

            var list = JsonConvert.DeserializeObject<List<LMSUserResponseModel>>(res.Result?.ToString() ?? "[]");
            

            if (list == null || !list.Any())
            {
                _logger.LogWarning("No user found with the given parameters: {@Params}", parameters);
                throw new HttpStatusCodeException(404, "No user found");
            }

            _logger.LogInformation("user list retrieved successfully.");
            return res;
        }
        catch (HttpStatusCodeException ex)
        {
            _logger.LogWarning("Known error occurred while fetching users list: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching users list with parameters: {@Params}", parameters);
            throw new HttpStatusCodeException(500, "An unexpected error occurred while fetching the user list");
        }
    }

    
    //get user by sid
    public async Task<LMSUserResponseModel?> GetuserBySID(string usersid)
    {
        _logger.LogInformation("Fetching book with SID: {Sid}", usersid);

        try
        {
            string sqlQuery = "SP_GetUserBySId {0}";
            object[] param = { usersid };

            var jsonResult = await _spContext.ExecuteStoreProcedure(sqlQuery, param);

            if (string.IsNullOrEmpty(jsonResult))
            {
                _logger.LogWarning("No book found with SID: {Sid}", usersid);
                throw new HttpStatusCodeException(404, $"Book with SID '{usersid}' not found");
            }

            var book = JsonConvert.DeserializeObject<LMSUserResponseModel>(jsonResult);

            if (book == null)
            {
                _logger.LogWarning("Deserialization failed or empty result for book SID: {Sid}", usersid);
                throw new HttpStatusCodeException(500, "Failed to parse book details");
            }

            _logger.LogInformation("Successfully retrieved book: (SID: {Sid})", usersid);
            return book;
        }
        catch (HttpStatusCodeException ex)
        {
            _logger.LogWarning("Known error occurred while fetching book with SID {Sid}: {Message}", usersid, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching book with SID: {Sid}", usersid);
            throw new HttpStatusCodeException(500, "An unexpected error occurred while fetching the book");
        }
    }
    
    //insert user 
  public async Task<List<LMSUserResponseModel>> InsertUser(List<UserSignupRequestModel> users)
{
    _logger.LogInformation("Inserting {Count} new users", users?.Count ?? 0);

    try
    {
        if (users == null || users.Count == 0)
        {
            _logger.LogWarning("Insert failed: User list cannot be empty.");
            throw new HttpStatusCodeException(400, "User list cannot be empty.");
        }

        List<User> userList = new List<User>();

        foreach (var user in users)
        {
            if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
            {
                _logger.LogWarning("Insert failed: Missing information for request {@User}", user);
                throw new HttpStatusCodeException(400, "Name, Email and Password are required.");
            }
            
            var existingEmail = await _unitOfWork.GetRepository<User>()
                .GetAllAsync(u => u.Email == user.Email);

            if (existingEmail.Any())
            {
                _logger.LogWarning("Insert failed: user with the same email is present - {Email}", user.Email);
                throw new HttpStatusCodeException(409, $"A user with the Email '{user.Email}' already exists.");
            }
            
            string prefix = user.Role?.ToLower() == "admin" ? "ADM" : "USR";
            string userSid = prefix + Guid.NewGuid();

            var u = new User
            {
                UserSid = userSid,
                Name = user.Name,
                Email = user.Email,
                PasswordHash = PasswordHelper.HashPassword(user.Password), 
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Role = user.Role,
                Status = (int)Enums.Active,
                CreatedAt = DateTime.Now,
                CreatedBy = "System"
            };

            userList.Add(u);
        }

        await _unitOfWork.GetRepository<User>().InsertAsync(userList);
        await _unitOfWork.CommitAsync();

        var resUser = userList.Select(u => new LMSUserResponseModel()
        {
            UserSid = u.UserSid,
            Name = u.Name,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            Address = u.Address,
            Role = u.Role,
            Status = u.Status,
            CreatedAt = u.CreatedAt,
        }).ToList();

        _logger.LogInformation("Successfully inserted {Count} users", resUser.Count);
        return resUser;
    }
    catch (HttpStatusCodeException ex)
    {
        _logger.LogWarning("Known error occurred while inserting users: {Message}", ex.Message);
        throw;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error occurred while inserting users");
        throw new HttpStatusCodeException(500, "An unexpected error occurred while inserting users");
    }
}

  //login user 
//   public async Task<LoginResponseModel> LoginUser(LoginRequestModel model)
// {
//     _logger.LogInformation("Attempting login for email: {Email}", model.Email);
//
//     try
//     {
//         if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
//         {
//             _logger.LogWarning("Login failed: Email and Password are required");
//             throw new HttpStatusCodeException(400, "Email and Password are required.");
//         }
//         
//         var user = (await _unitOfWork.GetRepository<User>()
//             .GetAllAsync(u => u.Email == model.Email && u.Status == (int)Enums.Active))
//             .FirstOrDefault();
//
//         if (user == null)
//         {
//             _logger.LogWarning("Login failed: User not found for email: {Email}", model.Email);
//             throw new HttpStatusCodeException(401, "Invalid email or password.");
//         }
//         
//         if (!PasswordHelper.VerifyPassword(model.Password, user.PasswordHash))
//         {
//             _logger.LogWarning("Login failed: Invalid password for email: {Email}", model.Email);
//             throw new HttpStatusCodeException(401, "Invalid email or password.");
//         }
//         
//         var response = await _authRepository.LoginUser(model);
//         
//         var cookieOptions = new CookieOptions
//         {
//             HttpOnly = true,
//             Secure = true, // only over HTTPS in production
//             SameSite = SameSiteMode.Strict,
//             Expires = DateTime.UtcNow.AddDays(7) // adjust expiry as needed
//         };
//
//         Response.Cookies.Append("jwt", response.Token, cookieOptions);
//
//         return Ok(new
//         {
//             response.UserSid,
//             response.Name,
//             response.Email,
//             response.Role,
//             message = "Login successful"
//         });
//
//         _logger.LogInformation("Login successful for email: {Email}", model.Email);
//         return response;
//     }
//     catch (HttpStatusCodeException)
//     {
//         throw;
//     }
//     catch (Exception ex)
//     {
//         _logger.LogError(ex, "Unexpected error occurred during login for email: {Email}", model.Email);
//         throw new HttpStatusCodeException(500, "An unexpected error occurred during login.");
//     }
// }

    //Delete User
    public async Task<bool> Deleteuser(string userSid)
    {
        _logger.LogInformation("Deleting (marking as unavailable) user with SID: {Sid}", userSid);
        try
        {
            var users = await _unitOfWork.GetRepository<User>().GetAllAsync();
            var user = users.FirstOrDefault(x =>
                x.UserSid == userSid && x.Status == (int)Enums.Active);

            if (user == null)
            {

                _logger.LogWarning("User not found for deletion. SID: {Sid}", userSid);
                throw new HttpStatusCodeException(400, "User not found for deletion");
            }

            user.Status = (int)Enums.Inactive;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Book successfully marked as unavailable. SID: {Sid}", userSid);
            return true;
        }
        catch (HttpStatusCodeException ex)
        {
            _logger.LogWarning("Deleting failed Book with the Sid:{Sid}", userSid);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while deleting (marking unavailable) book with SID: {Sid}", userSid);
            throw;
        }
    }
    
    
    }
    