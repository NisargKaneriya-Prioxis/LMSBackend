using LM.Model.ResponseModel;

namespace LM.Services.Repositories.Interface;

public interface IReturnBookRepository
{
    Task<LMSBorrowedBookResponseModel> ReturnBook(string borrowedSid, string userSid);
}