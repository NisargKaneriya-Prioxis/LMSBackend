
using LM.Model.CommonModel;
using LM.Model.RequestModel;
using LM.Model.ResponseModel;

namespace LM.Services.Repositories.Interface;

public interface IBookRepository
{
    Task<Page> List(Dictionary<string, object> parameters);

    Task<LMSBookResponseModel?> GetbookBySid(string booksid);

    Task<List<LMSBookResponseModel>> InsertBook(string CategorySID,List<LMSBookRequestModel> books);

    Task<LMSBookResponseModel?> UpdateBook(string bookSid, LMSBookRequestModel book, string CategorySID);

   Task<bool> BorrowedBook(string bookSid, string Isbn);

   Task<bool> ReturnBook(string bookSid, string Isbn);

   Task<bool> Deletebook(string bookSid);
}