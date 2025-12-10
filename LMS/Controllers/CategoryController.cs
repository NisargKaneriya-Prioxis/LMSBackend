using EvaluationAPI.Controllers;
using LM.Model.RequestModel;
using LM.Model.ResponseModel;
using LM.Services.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LMS.Controllers;

public class CategoryController : BaseController
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ICategoryRepository CategoryRepository, ILogger<CategoryController> logger)
    {
        _categoryRepository = CategoryRepository;
        _logger = logger;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("getallcategory")]
    public async Task<ActionResult<IEnumerable<LMSCategoryResponseModel>>> GetallCategory([FromQuery] SearchRequestModel model)
    {
        _logger.LogInformation("Fetching all Catagories with search params: {@SearchRequest}", model);

        var parameters = FillParamesFromModel(model);
        var list = await _categoryRepository.List(parameters);
        

        if (list != null)
        {
            var result = JsonConvert.DeserializeObject<List<LMSCategoryResponseModel>>(list.Result?.ToString() ?? "[]") ?? [];
            list.Result = result;
            _logger.LogInformation("Retrieved {Count} Category ", result.Count);
            return Ok(BindSearchResult(list, model, "Category list"));
        }

        _logger.LogWarning("No Category found matching search parameters");
        return NoContent();
    }

    
    [Authorize(Roles = "Admin")]
    //insert category 
    [HttpPost("addcategory")]
    public async Task<ActionResult<IEnumerable<LMSCategoryResponseModel>>> InsertCategory([FromBody]
        List<LMSCategoryRequestModel> categories)
    {   
        _logger.LogInformation("Inserting new category");
        List<LMSCategoryResponseModel> createdCategory = await _categoryRepository.InsertCategory(categories);
        if (createdCategory == null)
        {
            _logger.LogInformation("Failed to create the new category category");
            return BadRequest();
        }
        _logger.LogInformation("Book created successfully");
        return Ok(createdCategory);
    }
}   
    
    