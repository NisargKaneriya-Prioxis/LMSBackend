using LM.Model.CommonModel;
using LM.Model.RequestModel;
using LM.Model.ResponseModel;

namespace LM.Services.Repositories.Interface;

public interface ICategoryRepository
{
    Task<Page> List(Dictionary<string, object> parameters);

    Task<List<LMSCategoryResponseModel>> InsertCategory(List<LMSCategoryRequestModel> categories);
}