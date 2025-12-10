using LM.Model.CommonModel;
using LM.Model.RequestModel;
using LM.Model.ResponseModel;

namespace LM.Services.Repositories.Interface;

public interface IBorrowedBookRepository
{
    Task<Page> List(Dictionary<string, object> parameters);

    Task<List<LMSBorrowedBookResponseModel>> InsertBorrowedBook(string booksid, string usersid, List<LMSBorrowedBookRequestModel> borrowedBooks);
    Task<Page> StudentBorrowedList(Dictionary<string, object> parameters);
}