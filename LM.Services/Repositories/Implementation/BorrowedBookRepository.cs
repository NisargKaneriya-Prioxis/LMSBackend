using EA.Services.RepositoryFactory;
using LM.Common;
using LM.Model.CommonModel;
using LM.Model.Models.MyLMSDB;
using LM.Model.RequestModel;
using LM.Model.ResponseModel;
using LM.Model.SpDbContext;
using LM.Services.Repositories.Interface;
using LM.Services.UnitOfWork;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LM.Services.Repositories.Implementation;

public class BorrowedBookRepository : IBorrowedBookRepository
{
    private readonly LMSDbContext _context;
    private readonly ILogger<BorrowedBookRepository> _logger;
    private readonly LMSSpContext _spContext;
    private readonly IUnitOfWork _unitOfWork;

    public BorrowedBookRepository(
        LMSDbContext context,
        IUnitOfWork unitOfWork,
        ILogger<BorrowedBookRepository> logger,
        LMSSpContext spContext)
    {
        _context = context;
        _logger = logger;
        _spContext = spContext;
        _unitOfWork = unitOfWork;
    }

    //Get ALL Borrowed book for admin 
    public async Task<Page> List(Dictionary<string, object> parameters)
    {
        _logger.LogInformation("Fetching list of Borrowed books with parameters: {@Params}", parameters);

        try
        {
            var xmlParam = CommonHelper.DictionaryToXml(parameters, "Search");
            string sqlQuery = "SP_GetAllBorrowed {0}";
            object[] param = { xmlParam };

            var res = await _spContext.ExecutreStoreProcedureResultList(sqlQuery, param);

            var list = JsonConvert.DeserializeObject<List<LMSCategoryResponseModel>>(res.Result?.ToString() ?? "[]");


            if (list == null || !list.Any())
            {
                _logger.LogWarning("No Borrowed books found with the given parameters: {@Params}", parameters);
                throw new HttpStatusCodeException(404, "No Borrowed books found");
            }

            _logger.LogInformation("Borrowed Books list retrieved successfully.");
            return res;
        }
        catch (HttpStatusCodeException ex)
        {
            _logger.LogWarning("Known error occurred while fetching Borrowed books list: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error occurred while fetching Borrowed books list with parameters: {@Params}", parameters);
            throw new HttpStatusCodeException(500,
                "An unexpected error occurred while fetching the borrowed sbook list");
        }
    }
    
    //get all borrowed book for students
    public async Task<Page> StudentBorrowedList(Dictionary<string, object> parameters)
    {
        _logger.LogInformation("Fetching list of Borrowed books with parameters: {@Params}", parameters);

        try
        {
            var xmlParam = CommonHelper.DictionaryToXml(parameters, "Search");
            string sqlQuery = "SP_GetAllBorrowedStudents {0}";
            object[] param = { xmlParam };

            var res = await _spContext.ExecutreStoreProcedureResultList(sqlQuery, param);

            var list = JsonConvert.DeserializeObject<List<LMSCategoryResponseModel>>(res.Result?.ToString() ?? "[]");


            if (list == null || !list.Any())
            {
                _logger.LogWarning("No Borrowed books found with the given parameters: {@Params}", parameters);
                throw new HttpStatusCodeException(404, "No Borrowed books found");
            }

            _logger.LogInformation("Borrowed Books list retrieved successfully.");
            return res;
        }
        catch (HttpStatusCodeException ex)
        {
            _logger.LogWarning("Known error occurred while fetching Borrowed books list: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error occurred while fetching Borrowed books list with parameters: {@Params}", parameters);
            throw new HttpStatusCodeException(500,
                "An unexpected error occurred while fetching the borrowed sbook list");
        }
    }

    //borrowedbook
    public async Task<List<LMSBorrowedBookResponseModel>> InsertBorrowedBook(string booksid, string usersid, List<LMSBorrowedBookRequestModel> borrowedBooks)
    {
        _logger.LogInformation("Inserting {Count} borrowed book records", borrowedBooks?.Count ?? 0);

        try
        {
            if (borrowedBooks == null || borrowedBooks.Count == 0)
            {
                _logger.LogWarning("Insert failed: Borrowed book list cannot be empty.");
                throw new HttpStatusCodeException(400, "Borrowed book list cannot be empty.");
            }

            List<BorrowedBook> borrowedList = new List<BorrowedBook>();

            foreach (var req in borrowedBooks)
            {
                var book = await _unitOfWork.GetRepository<Book>()
                    .SingleOrDefaultAsync(b => b.BookSid == booksid && b.Status == (int)Enums.Active);

                var user = await _unitOfWork.GetRepository<User>()
                    .SingleOrDefaultAsync(u => u.UserSid == usersid && u.Status == (int)Enums.Active);


                var borrowed = new BorrowedBook
                {
                    BorrowedSid = "BB" + Guid.NewGuid(),
                    UserId = user.UserId,
                    BookId = book.BookId,
                    IssueDate = DateTime.UtcNow,
                    DueDate = req.DueDate,
                    ReturnDate = null,
                    BorrowedBookStatus = (int)Enums.Borrowed,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = user.UserSid
                };

                borrowedList.Add(borrowed);
                
                book.AvailableQuantity = (int)book.AvailableQuantity-1 ;
                if (book.AvailableQuantity == 0)
                {
                    book.BorrowedStatus = (int)Enums.UnAvailable;
                
                
                }
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
            }

            await _unitOfWork.GetRepository<BorrowedBook>().InsertAsync(borrowedList);
            await _unitOfWork.CommitAsync();

            var resList = borrowedList.Select(b => new LMSBorrowedBookResponseModel
            {
                BorrowedSID = b.BorrowedSid,
                IssueDate = b.IssueDate,
                DueDate = b.DueDate,
                ReturnDate = b.ReturnDate,
                BorrowedBookStatus = b.BorrowedBookStatus,
            }).ToList();

            _logger.LogInformation("Successfully inserted {Count} borrowed book records", resList.Count);
            return resList;
        }
        catch (HttpStatusCodeException ex)
        {
            _logger.LogWarning("Known error occurred while inserting borrowed book: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while inserting borrowed book");
            throw new HttpStatusCodeException(500, "An unexpected error occurred while inserting borrowed books");
        }
    }
    
    //returnBook
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
