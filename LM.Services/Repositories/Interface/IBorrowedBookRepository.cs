using LM.Model.CommonModel;

namespace LM.Services.Repositories.Interface;

public interface IBorrowedBookRepository
{
    Task<Page> List(Dictionary<string, object> parameters);
}