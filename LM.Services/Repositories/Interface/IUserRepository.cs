using LM.Model.CommonModel;
using LM.Model.RequestModel;
using LM.Model.ResponseModel;

namespace LM.Services.Repositories.Interface;

public interface IUserRepository
{
    Task<Page> List(Dictionary<string, object> parameters);

    Task<LMSUserResponseModel?> GetuserBySID(string usersid);

    Task<List<LMSUserResponseModel>> InsertUser(List<UserSignupRequestModel> users);

    Task<LoginResponseModel> LoginUser(LoginRequestModel model);

    Task<bool> Deleteuser(string userSid);
}