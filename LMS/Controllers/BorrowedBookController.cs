using EvaluationAPI.Controllers;
using LM.Model.ResponseModel;
using LM.Services.Repositories.Interface;
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
}