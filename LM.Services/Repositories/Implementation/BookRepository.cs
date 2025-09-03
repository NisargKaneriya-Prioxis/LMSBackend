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

public class BookRepository : IBookRepository
{   
    private readonly LMSDbContext _context;
    private readonly ILogger<BookRepository> _logger;
    private readonly LMSSpContext _spContext;
    private readonly IUnitOfWork _unitOfWork;
    
    public BookRepository(
        LMSDbContext context,
        IUnitOfWork unitOfWork,
        ILogger<BookRepository> logger,
        LMSSpContext spContext)
    {
        _context = context;
        _logger = logger;
        _spContext = spContext;
        _unitOfWork = unitOfWork;
    }

    
    //Get ALL
    public async Task<Page> List(Dictionary<string, object> parameters)
    {
        _logger.LogInformation("Fetching list of books with parameters: {@Params}", parameters);

        try
        {
            var xmlParam = CommonHelper.DictionaryToXml(parameters, "Search");
            string sqlQuery = "SP_GetAllBook {0}";
            object[] param = { xmlParam };

            var res = await _spContext.ExecutreStoreProcedureResultList(sqlQuery, param);

            var list = JsonConvert.DeserializeObject<List<LMSBookResponseModel>>(res.Result?.ToString() ?? "[]");
            

            if (list == null || !list.Any())
            {
                _logger.LogWarning("No books found with the given parameters: {@Params}", parameters);
                throw new HttpStatusCodeException(404, "No books found");
            }

            _logger.LogInformation("Books list retrieved successfully.");
            return res;
        }
        catch (HttpStatusCodeException ex)
        {
            _logger.LogWarning("Known error occurred while fetching books list: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching books list with parameters: {@Params}", parameters);
            throw new HttpStatusCodeException(500, "An unexpected error occurred while fetching the book list");
        }
    }
    
    //Get by Sid
    public async Task<LMSBookResponseModel?> GetbookBySid(string booksid)
    {
        _logger.LogInformation("Fetching book with SID: {Sid}", booksid);

        try
        {
            string sqlQuery = "SP_GetBookBySID {0}";
            object[] param = { booksid };

            var jsonResult = await _spContext.ExecuteStoreProcedure(sqlQuery, param);

            if (string.IsNullOrEmpty(jsonResult))
            {
                _logger.LogWarning("No book found with SID: {Sid}", booksid);
                throw new HttpStatusCodeException(404, $"Book with SID '{booksid}' not found");
            }

            var book = JsonConvert.DeserializeObject<LMSBookResponseModel>(jsonResult);

            if (book == null)
            {
                _logger.LogWarning("Deserialization failed or empty result for book SID: {Sid}", booksid);
                throw new HttpStatusCodeException(500, "Failed to parse book details");
            }

            _logger.LogInformation("Successfully retrieved book: {Title} (SID: {Sid})", book.Title, booksid);
            return book;
        }
        catch (HttpStatusCodeException ex)
        {
            _logger.LogWarning("Known error occurred while fetching book with SID {Sid}: {Message}", booksid, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching book with SID: {Sid}", booksid);
            throw new HttpStatusCodeException(500, "An unexpected error occurred while fetching the book");
        }
    }
    
    
    //insert book
    public async Task<List<LMSBookResponseModel>> InsertBook(string CategorySID,List<LMSBookRequestModel> books)
{
    _logger.LogInformation("Inserting {Count} new books", books?.Count ?? 0);
    
    try
    {
        if (books == null || books.Count == 0)
        {
            _logger.LogWarning("Insert failed: Book list cannot be empty.");
            throw new HttpStatusCodeException(400, "Book list cannot be empty.");
        }

        List<Book> bookList = new List<Book>();

        foreach (var book in books)
        {
            if (string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Author))
            {
                _logger.LogWarning("Insert failed: Missing Title/Author for book request {@Book}", book);
                throw new HttpStatusCodeException(400, "Book Title and Author are required.");
            }
            var existingTitle = await _unitOfWork.GetRepository<Book>()
                .GetAllAsync(b => b.Title == book.Title);
            
            if (existingTitle.Any())
            {
                _logger.LogWarning("Insert failed: Duplicate Title detected - {Title}", book.Title);
                throw new HttpStatusCodeException(409, $"A book with the title '{book.Title}' already exists.");
            }
            
            if (!string.IsNullOrWhiteSpace(book.Isbn))
            {
                var existingIsbn = await _unitOfWork.GetRepository<Book>()
                    .GetAllAsync(b => b.Isbn == book.Isbn);
            
                if (existingIsbn.Any())
                {
                    _logger.LogWarning("Insert failed: Duplicate ISBN detected - {Isbn}", book.Isbn);
                    throw new HttpStatusCodeException(409, $"A book with the ISBN '{book.Isbn}' already exists.");
                }
            }

            var category = await _unitOfWork.GetRepository<Category>()
                .SingleOrDefaultAsync(c => c.CategorySid == CategorySID);
            
            var b = new Book
            {
                BookSid = "LIB" + Guid.NewGuid(),
                Author = book.Author,
                Title = book.Title,
                PublishYear = book.PublishYear,
                Isbn = book.Isbn,
                Edition=book.Edition,
                Language=book.Language,
                CategoryId= category.CategoryId,
                Quantity=book.Quantity,
                AvailableQuantity=book.AvailableQuantity,
                Publisher=book.Publisher,
                BorrowedStatus = (int)Enums.IsAvailable
            };

            bookList.Add(b);
        }

        await _unitOfWork.GetRepository<Book>().InsertAsync(bookList);
        await _unitOfWork.CommitAsync();

        var resBooks = bookList.Select(b => new LMSBookResponseModel()
        {
            BookSid = b.BookSid,
            Author = b.Author,
            Title = b.Title,
            PublishYear = b.PublishYear,
            Isbn = b.Isbn,
            Edition=b.Edition,
            Language=b.Language,
            CategoryId= b.CategoryId,
            Quantity=b.Quantity,
            AvailableQuantity=b.AvailableQuantity,
            Publisher=b.Publisher,
            BorrowedStatus = b.BorrowedStatus
        }).ToList();

        _logger.LogInformation("Successfully inserted {Count} books", resBooks.Count);
        return resBooks;
    }
    catch (HttpStatusCodeException ex)
    {
        _logger.LogWarning("Known error occurred while inserting books: {Message}", ex.Message);
        throw;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error occurred while inserting books");
        throw new HttpStatusCodeException(500, "An unexpected error occurred while inserting books");
    }
}
       
    //update book
   public async Task<LMSBookResponseModel?> UpdateBook(string bookSid, LMSBookRequestModel book, string CategorySID)
{
    _logger.LogInformation("Updating book with SID: {Sid}", bookSid);

    try
    {
        var existingBook = await _unitOfWork.GetRepository<Book>()
            .SingleOrDefaultAsync(x =>
                x.BookSid == bookSid &&
                x.BorrowedStatus == (int)Enums.IsAvailable &&
                x.Status == (int)Enums.Active);

        if (existingBook == null)
        {
            _logger.LogWarning("Book not found or unavailable for update. SID: {Sid}", bookSid);
            throw new HttpStatusCodeException(404, $"Book with SID '{bookSid}' not found or unavailable for update.");
        }
        
        var category = await _unitOfWork.GetRepository<Category>()
            .SingleOrDefaultAsync(c => c.CategorySid == CategorySID);

        if (category == null)
        {
            _logger.LogWarning("Category not found for SID: {CategorySid}", CategorySID);
            throw new HttpStatusCodeException(404, $"Category with SID '{CategorySID}' not found.");
        }
        existingBook.Title = book.Title;
        existingBook.Author = book.Author;
        existingBook.Isbn = book.Isbn;
        existingBook.PublishYear = book.PublishYear;
        existingBook.Edition = book.Edition;
        existingBook.Language = book.Language;
        existingBook.CategoryId = category.CategoryId; 
        existingBook.Quantity = book.Quantity;
        existingBook.AvailableQuantity = book.AvailableQuantity;
        existingBook.Publisher = book.Publisher;
        existingBook.BorrowedStatus = (int)Enums.IsAvailable;
        existingBook.Status = (int)Enums.Active;
        existingBook.ModifiedAt = DateTime.Now;

        _unitOfWork.GetRepository<Book>().Update(existingBook);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Book updated successfully with SID: {Sid}", bookSid);

        return new LMSBookResponseModel
        {
            BookSid = existingBook.BookSid,
            Title = existingBook.Title,
            Author = existingBook.Author,
            Isbn = existingBook.Isbn,
            PublishYear = existingBook.PublishYear,
            Edition = existingBook.Edition,
            Language = existingBook.Language,
            CategoryId = existingBook.CategoryId,  
            CategoryName = category.CategoryName,  
            Quantity = existingBook.Quantity,
            AvailableQuantity = existingBook.AvailableQuantity,
            Publisher = existingBook.Publisher,
            BorrowedStatus = existingBook.BorrowedStatus,
            Status = existingBook.Status
        };
    }
    catch (HttpStatusCodeException)
    {
        throw;
    }
    catch (Exception e)
    {
        _logger.LogError(e, "An unexpected error occurred while updating book with SID: {Sid}", bookSid);
        throw new HttpStatusCodeException(500, "An unexpected error occurred while updating books");
    }
}

    //Borrowed Book
    public async Task<bool> BorrowedBook(string bookSid, string Isbn )
    {
        _logger.LogInformation("Borrowing book with SID: {Sid} and ISBN: {isbn}", bookSid, Isbn);

        try
        {
            var books = await _unitOfWork.GetRepository<Book>().GetAllAsync();

            var book = books.FirstOrDefault(x =>
                x.BookSid == bookSid &&
                x.Isbn == Isbn &&
                x.BorrowedStatus == (int)Enums.IsAvailable &&
                x.Status == (int)Enums.Active);

            if (book == null)
            {
                _logger.LogWarning("Book with SID: {Sid} and ISBN: {isbn} not available or already borrowed", bookSid, Isbn);
                throw new HttpStatusCodeException(404, $"Book with SID '{bookSid}' and ISBN '{Isbn}' not available or already borrowed.");
            }
                
            book.AvailableQuantity = (int)book.AvailableQuantity-1 ;
            if (book.AvailableQuantity == 0)
            {
                book.BorrowedStatus = (int)Enums.UnAvailable;
                
                
            }
            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Book with SID: {Sid} and ISBN: {isbn} successfully borrowed", bookSid, Isbn);
            return true;
        }
        catch (HttpStatusCodeException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error occurred while borrowing book with SID: {Sid} and ISBN: {isbn}", bookSid, Isbn);
            throw new HttpStatusCodeException(500, "An unexpected error occurred while borrowing the book. Please try again later.");
        }
    }
    
    //Return Book 
    public async Task<bool> ReturnBook(string bookSid, string Isbn)
    {
        _logger.LogInformation("Return book with SID: {Sid} and ISBN: {isbn}", bookSid, Isbn);

        try
        {
            var books = await _unitOfWork.GetRepository<Book>().GetAllAsync();
            var book = books.FirstOrDefault(x =>
                x.BookSid == bookSid && 
                x.Isbn == Isbn && 
                // x.BorrowedStatus == (int)Enums.UnAvailable &&
                x.Status == (int)Enums.Active);

            if (book == null)
            {
                _logger.LogWarning("Book with SID: {Sid} and ISBN: {isbn} not found or already available in the library", bookSid, Isbn);
                throw new HttpStatusCodeException(404, $"Book with SID: {bookSid} and ISBN: {Isbn} not found or already returned.");
            }

            if (book.AvailableQuantity < book.Quantity)
            {
                book.AvailableQuantity = (int)book.AvailableQuantity+1 ;
            }
            else
            {
                throw new ("Book is already Present");
            }
            
            if (book.AvailableQuantity>0)
            {
                book.BorrowedStatus = (int)Enums.IsAvailable;
            }

            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Book with SID: {Sid} and ISBN: {isbn} successfully returned", bookSid, Isbn);
            return true;
        }
        catch (HttpStatusCodeException) 
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error occurred while returning book with SID: {Sid} and ISBN: {isbn}", bookSid, Isbn);
            throw new HttpStatusCodeException(500, "An unexpected error occurred while returning the book.");
        }
    }
    
    
    //Delete book
    public async Task<bool> Deletebook(string bookSid)
    {
        _logger.LogInformation("Deleting (marking as unavailable) book with SID: {Sid}", bookSid);
        try
        {
            var books = await _unitOfWork.GetRepository<Book>().GetAllAsync();
            var book = books.FirstOrDefault(x =>
                x.BookSid == bookSid && x.BorrowedStatus == (int)Enums.IsAvailable &&
                x.Status == (int)Enums.Active);

            if (book == null)
            {

                _logger.LogWarning("Book not found for deletion. SID: {Sid}", bookSid);
                throw new HttpStatusCodeException(400, "Book not found for deletion");
            }

            book.Status = (int)Enums.Inactive;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Book successfully marked as unavailable. SID: {Sid}", bookSid);
            return true;
        }
        catch (HttpStatusCodeException ex)
        {
            _logger.LogWarning("Deleting failed Book with the Sid:{Sid}", bookSid);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while deleting (marking unavailable) book with SID: {Sid}", bookSid);
            throw;
        }
    }

}