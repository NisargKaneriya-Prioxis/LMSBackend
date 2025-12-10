using EvaluationAPI.Controllers;
using LM.Model.RequestModel;
using LM.Model.ResponseModel;
using LM.Services.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LMS.Controllers;

public class BorrowedBookController : BaseController
{
    private readonly IBorrowedBookRepository _borrowedbookrepository;
    private readonly ILogger<BorrowedBookController> _logger;

    public BorrowedBookController(IBorrowedBookRepository BorrowedBookRepository, ILogger<BorrowedBookController> logger)
    {
        _borrowedbookrepository = BorrowedBookRepository;
        _logger = logger;
    }
    
    [HttpGet("getallborrowedbook")]
    public async Task<ActionResult<IEnumerable<LMSBorrowedBookResponseModel>>> GetallBorrowedBook([FromQuery] SearchRequestModel model)
    {
        _logger.LogInformation("Fetching all Borrowed Book with search params: {@SearchRequest}", model);

        var parameters = FillParamesFromModel(model);
        var list = await _borrowedbookrepository.List(parameters);
        

        if (list != null)
        {
            var result = JsonConvert.DeserializeObject<List<LMSBorrowedBookResponseModel>>(list.Result?.ToString() ?? "[]") ?? [];
            list.Result = result;
            _logger.LogInformation("Retrieved {Count} Borrowed Book  ", result.Count);
            return Ok(BindSearchResult(list, model, "Borrowed Book list"));
        }

        _logger.LogWarning("No Borrowed Book found matching search parameters");
        return NoContent();
    }
    
    [HttpGet("getallborrowedbookStudent")]
    public async Task<ActionResult<IEnumerable<LMSBorrowedBookResponseModel>>> GetallBorrowedBookStudent([FromQuery] SearchRequestModel model)
    {
        _logger.LogInformation("Fetching all Borrowed Book fro students with search params: {@SearchRequest}", model);

        var parameters = FillParamesFromModel(model);
        var list = await _borrowedbookrepository.StudentBorrowedList(parameters);
        

        if (list != null)
        {
            var result = JsonConvert.DeserializeObject<List<LMSBorrowedBookResponseModel>>(list.Result?.ToString() ?? "[]") ?? [];
            list.Result = result;
            _logger.LogInformation("Retrieved {Count} Borrowed Book for students  ", result.Count);
            return Ok(BindSearchResult(list, model, "Borrowed Book list for students"));
        }

        _logger.LogWarning("No Borrowed Book found for students matching search parameters");
        return NoContent();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("Inserborrowedbook")]
    public async Task<ActionResult<List<LMSBorrowedBookResponseModel>>> InsertBorrowedBook([FromQuery] string booksid, [FromQuery] string usersid,[FromBody] List<LMSBorrowedBookRequestModel> borrowedbook)
    {
        List<LMSBorrowedBookResponseModel> createdBook = await _borrowedbookrepository.InsertBorrowedBook(booksid,usersid,borrowedbook);
        if (createdBook == null)
        {
            _logger.LogInformation("Failed to create book: {@BookData}", borrowedbook);
            return BadRequest();
        }

        _logger.LogInformation("Book created successfully");
        return Ok(createdBook);
    }
    

}