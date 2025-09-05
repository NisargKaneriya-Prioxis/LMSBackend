using LM.Model.CommonModel;

namespace LM.Services.Repositories.Interface;

public interface ICategoryRepository
{
    Task<Page> List(Dictionary<string, object> parameters);
}