using LM.Model.CommonModel;
using LM.Model.RequestModel;
using LM.Model.ResponseModel;

namespace EA.Services.RepositoryFactory;

public interface IRequestBookRepository
{
    Task<Page> List(Dictionary<string, object> parameters);

    Task<List<LMSRequestBookResponseModel>> InsertBookRequest(string UserSid, string BookSid,
        List<LMSRequestBookRequestModel> Requestbooks);
}