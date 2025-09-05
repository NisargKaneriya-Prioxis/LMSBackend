using EvaluationAPI.Controllers;
using LM.Model.RequestModel;
using LM.Model.ResponseModel;
using LM.Services.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LMS.Controllers;

public class BookController : BaseController
{   
    private readonly IBookRepository _BookRepository;
    private readonly ILogger<BookController> _logger;

    public BookController(IBookRepository BookRepository, ILogger<BookController> logger)
    {
        _BookRepository = BookRepository;
        _logger = logger;
    }
    
    
    //get all controller 
    [HttpGet("getallbook")]
    public async Task<ActionResult<IEnumerable<LMSBookResponseModel>>> GetallBooks([FromQuery] SearchRequestModel model)
    {
        _logger.LogInformation("Fetching all books with search params: {@SearchRequest}", model);

        var parameters = FillParamesFromModel(model);
        var list = await _BookRepository.List(parameters);
        

        if (list != null)
        {
            var result = JsonConvert.DeserializeObject<List<LMSBookResponseModel>>(list.Result?.ToString() ?? "[]") ?? [];
            list.Result = result;
            _logger.LogInformation("Retrieved {Count} books", result.Count);
            return Ok(BindSearchResult(list, model, "book list"));
        }

        _logger.LogWarning("No books found matching search parameters");
        return NoContent();
    }
    
    
    //get by sid controller 
    [HttpGet("dynamic/{booksid}")]
    public async Task<ActionResult<LMSBookResponseModel>> GetByBookSID([FromRoute] string booksid)
    {
        _logger.LogInformation("Fetching book with SID: {Sid}", booksid);

        var book = await _BookRepository.GetbookBySid(booksid);

        if (book == null)
        {
            _logger.LogWarning("Book not found with SID: {Sid}", booksid);
            return NotFound(new { message = $"Book with SID '{booksid}' not found" });
        }

        _logger.LogInformation("Successfully retrieved book: {Title} (SID: {Sid})", book.Title, booksid);
        return Ok(book);
    }
    
    // [Authorize(Roles = "Admin")]
    //insert controller 
    [HttpPost("InsertBook")]
    public async Task<ActionResult<List<LMSBookResponseModel>>> InsertBook(string CategorySID,[FromBody] List<LMSBookRequestModel> book)
    {
        List<LMSBookResponseModel> createdBook = await _BookRepository.InsertBook(CategorySID,book);
        if (createdBook == null)
        {
            _logger.LogInformation("Failed to create book: {@BookData}", book);
            return BadRequest();
        }

        _logger.LogInformation("Book created successfully");
        return Ok(createdBook);
    }
    
    // [Authorize(Roles = "Admin")]
    //Update controller 
    [HttpPost("updateBook/{BookSID}")]
    public async Task<ActionResult<LMSBookResponseModel>> UpdateBook([FromRoute] string CategorySID,[FromBody] LMSBookRequestModel model, [FromRoute] string booksid)
    {
        var book = await _BookRepository.UpdateBook(CategorySID,model,booksid);
        if (book != null)
        {
            _logger.LogInformation("Book with SID {BookSID} updated successfully.", booksid);
            return Ok(book);
        }

        _logger.LogInformation("Book with SID {BookSID} not found for update.", booksid);
        return NotFound();
    }
    
    // [Authorize(Roles = "Admin")]
    //delete Book
    [HttpDelete("deletebook/{bookSid}")]
    public async Task<ActionResult> DeleteBook([FromRoute] string bookSid)
    {
        _logger.LogInformation("Request to delete book with SID: {Sid}", bookSid);

        var result = await _BookRepository.Deletebook(bookSid);

        if (!result)
        {
            _logger.LogWarning("Book not found for deletion. SID: {Sid}", bookSid);
            return NotFound($"Book with SID '{bookSid}' not found.");
        }

        _logger.LogInformation("Successfully deleted book with SID: {Sid}", bookSid);
        return Ok("Book deleted.");
    }
}