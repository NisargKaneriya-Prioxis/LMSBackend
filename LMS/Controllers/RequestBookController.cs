using EA.Services.RepositoryFactory;
using EvaluationAPI.Controllers;
using LM.Model.RequestModel;
using LM.Model.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LMS.Controllers;

public class RequestBookController : BaseController
{
    private readonly IRequestBookRepository _requestbookRepository;
    private readonly ILogger<RequestBookController> _logger;

    public RequestBookController(IRequestBookRepository RequestBookRepository, ILogger<RequestBookController> logger)
    {
        _requestbookRepository = RequestBookRepository;
        _logger = logger;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("getallrequestbooks")]
    public async Task<ActionResult<IEnumerable<LMSRequestBookResponseModel>>> GetallRequestBook([FromQuery] SearchRequestModel model)
    {
        _logger.LogInformation("Fetching all Request Book with search params: {@SearchRequest}", model);

        var parameters = FillParamesFromModel(model);
        var list = await _requestbookRepository.List(parameters);
        

        if (list != null)
        {
            var result = JsonConvert.DeserializeObject<List<LMSRequestBookResponseModel>>(list.Result?.ToString() ?? "[]") ?? [];
            list.Result = result;
            _logger.LogInformation("Retrieved {Count} Request Book ", result.Count);
            return Ok(BindSearchResult(list, model, "RequestBook list"));
        }

        _logger.LogWarning("No RequestBook found matching search parameters");
        return NoContent();
    }
    
    //insert book request controller 
    [HttpPost("InsertBookRequest")]
    public async Task<ActionResult<List<LMSRequestBookResponseModel>>> InsertBookRequest(string UserSID,string BookSID,[FromBody] List<LMSRequestBookRequestModel> bookrequest)
    {
        List<LMSRequestBookResponseModel> createdBookRequest = await _requestbookRepository.InsertBookRequest(UserSID,BookSID,bookrequest);
        if (createdBookRequest == null)
        {
            _logger.LogInformation("Failed to create book Request: {@BookData}", bookrequest);
            return BadRequest();
        }

        _logger.LogInformation("Book Request created successfully");
        return Ok(createdBookRequest);
    }
  

}