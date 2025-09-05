using EA.Services.RepositoryFactory;
using LM.Common;
using LM.Model.CommonModel;
using LM.Model.Models.MyLMSDB;
using LM.Model.ResponseModel;
using LM.Model.SpDbContext;
using LM.Services.Repositories.Interface;
using LM.Services.UnitOfWork;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LM.Services.Repositories.Implementation;

public class CategoryRepository : ICategoryRepository
{
    private readonly LMSDbContext _context;
    private readonly ILogger<CategoryRepository> _logger;
    private readonly LMSSpContext _spContext;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryRepository(
        LMSDbContext context,
        IUnitOfWork unitOfWork,
        ILogger<CategoryRepository> logger,
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
        _logger.LogInformation("Fetching list of Catagories with parameters: {@Params}", parameters);

        try
        {
            var xmlParam = CommonHelper.DictionaryToXml(parameters, "Search");
            string sqlQuery = "SP_GetAllCategory {0}";
            object[] param = { xmlParam };

            var res = await _spContext.ExecutreStoreProcedureResultList(sqlQuery, param);

            var list = JsonConvert.DeserializeObject<List<LMSCategoryResponseModel>>(res.Result?.ToString() ?? "[]");
            

            if (list == null || !list.Any())
            {
                _logger.LogWarning("No Categories found with the given parameters: {@Params}", parameters);
                throw new HttpStatusCodeException(404, "No categories found");
            }

            _logger.LogInformation("categories list retrieved successfully.");
            return res;
        }
        catch (HttpStatusCodeException ex)
        {
            _logger.LogWarning("Known error occurred while fetching Categories list: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching Categories list with parameters: {@Params}", parameters);
            throw new HttpStatusCodeException(500, "An unexpected error occurred while fetching the Categories list");
        }
    }

}