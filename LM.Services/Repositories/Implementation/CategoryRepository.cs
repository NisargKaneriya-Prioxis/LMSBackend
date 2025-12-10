using System.Linq.Expressions;
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

    // insert category
    public async Task<List<LMSCategoryResponseModel>> InsertCategory(List<LMSCategoryRequestModel> categories)
    {
        _logger.LogInformation("Fetching list of categories with parameters: {@Params}", categories);

        try
        {
            if (categories == null || categories.Count == 0)
            {
                _logger.LogWarning("Insert failed: Categories list cannot be empty.");
                throw new HttpStatusCodeException(400, "Categories list cannot be empty.");
            }

            List<Category> CategoryList = new List<Category>();

            foreach (var c in categories)
            {
                if (string.IsNullOrWhiteSpace(c.CategoryName))
                {
                    _logger.LogWarning("Insert failed: Missing information for request {@Categories}", categories);
                    throw new HttpStatusCodeException(400, "Category name cannot be empty.");
                }

                var existingCategoryName = await _unitOfWork.GetRepository<Category>()
                    .GetAllAsync(x => x.CategoryName == c.CategoryName);

                if (existingCategoryName.Any())
                {
                    _logger.LogWarning("Insert failed: category Name is present - {CategoryName}", c.CategoryName);
                    throw new HttpStatusCodeException(409, $"category Name is already present - {c.CategoryName}");
                }

                var cat = new Category
                {
                    CategorySid = "CAT" + Guid.NewGuid(),
                    CategoryName = c.CategoryName,
                };

                CategoryList.Add(cat);
            }

            await _unitOfWork.GetRepository<Category>().InsertAsync(CategoryList);
            await _unitOfWork.CommitAsync();

            var rescategory = CategoryList.Select(c => new LMSCategoryResponseModel()
            {
                CategorySid = c.CategorySid,
                CategoryName = c.CategoryName,
            }).ToList();

            _logger.LogInformation("Successfully inserted Category");
            return rescategory;
        }
        catch (HttpStatusCodeException ex)
        {
            _logger.LogWarning("Known error occurred while inserting category: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while inserting Category");
            throw new HttpStatusCodeException(500, "An unexpected error occurred while inserting Category");
        }
    }
    
    

}