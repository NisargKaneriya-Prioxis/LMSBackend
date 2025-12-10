using LM.Common;
using LM.Model.Models.MyLMSDB;
using LM.Model.ResponseModel;
using LM.Model.SpDbContext;
using LM.Services.Repositories.Interface;
using LM.Services.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace LM.Services.Repositories.Implementation;

public class ReturnBookRepository : IReturnBookRepository
{
    private readonly LMSDbContext _context;
    private readonly ILogger<ReturnBookRepository> _logger;
    private readonly LMSSpContext _spContext;
    private readonly IUnitOfWork _unitOfWork;

    public ReturnBookRepository(
        LMSDbContext context,
        IUnitOfWork unitOfWork,
        ILogger<ReturnBookRepository> logger,
        LMSSpContext spContext)
    {
        _context = context;
        _logger = logger;
        _spContext = spContext;
        _unitOfWork = unitOfWork;
    }
    
    //update the return book 
     public async Task<LMSBorrowedBookResponseModel> ReturnBook(string borrowedSid, string userSid)
{
    _logger.LogInformation("Returning book for BorrowedSID: {BorrowedSid}, UserSID: {UserSid}", borrowedSid, userSid);

    try
    {
        var borrowedRecord = await _unitOfWork.GetRepository<BorrowedBook>()
            .SingleOrDefaultAsync(b => b.BorrowedSid == borrowedSid && b.BorrowedBookStatus == (int)Enums.Borrowed);

        if (borrowedRecord == null)
        {
            _logger.LogWarning("Return failed: No active borrowed record found for BorrowedSID: {BorrowedSid}", borrowedSid);
            throw new HttpStatusCodeException(404, "Borrowed record not found or already returned.");
        }

        var user = await _unitOfWork.GetRepository<User>()
            .SingleOrDefaultAsync(u => u.UserSid == userSid && u.Status == (int)Enums.Active);

        if (user == null)
        {
            _logger.LogWarning("Return failed: User not found with SID: {UserSid}", userSid);
            throw new HttpStatusCodeException(404, "User not found.");
        }

        var book = await _unitOfWork.GetRepository<Book>()
            .SingleOrDefaultAsync(b => b.BookId == borrowedRecord.BookId && b.Status == (int)Enums.Active);

        if (book == null)
        {
            _logger.LogWarning("Return failed: Book not found with ID: {BookId}", borrowedRecord.BookId);
            throw new HttpStatusCodeException(404, "Book not found.");
        }
        borrowedRecord.ReturnDate = DateTime.UtcNow;
        borrowedRecord.BorrowedBookStatus = (int)Enums.Unborrowed;
        borrowedRecord.ReturnDate = DateTime.UtcNow;
        borrowedRecord.ModifiedBy = user.UserSid;
        book.AvailableQuantity = (int)book.AvailableQuantity + 1;
        if (book.AvailableQuantity > 0)
        {
            book.BorrowedStatus = (int)Enums.IsAvailable;
        }

        _context.Books.Update(book);
        _context.BorrowedBooks.Update(borrowedRecord);
        await _unitOfWork.CommitAsync();

        var response = new LMSBorrowedBookResponseModel
        {
            BorrowedSID = borrowedRecord.BorrowedSid,
            IssueDate = borrowedRecord.IssueDate,
            DueDate = borrowedRecord.DueDate,
            ReturnDate = borrowedRecord.ReturnDate,
            BorrowedBookStatus = borrowedRecord.BorrowedBookStatus
        };

        _logger.LogInformation("Book successfully returned for BorrowedSID: {BorrowedSid}", borrowedSid);
        return response;
    }
    catch (HttpStatusCodeException ex)
    {
        _logger.LogWarning("Known error occurred while returning book: {Message}", ex.Message);
        throw;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error occurred while returning book");
        throw new HttpStatusCodeException(500, "An unexpected error occurred while returning book");
    }
}

}