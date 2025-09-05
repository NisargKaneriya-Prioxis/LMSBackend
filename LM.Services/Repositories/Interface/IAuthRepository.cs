using LM.Model.RequestModel;
using LM.Model.ResponseModel;

namespace LM.Services.Repositories.Interface;

public interface IAuthRepository
{
    Task<LoginResponseModel> LoginUser(LoginRequestModel model);
}