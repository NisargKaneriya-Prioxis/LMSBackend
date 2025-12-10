using EA.Services.RepositoryFactory;
using LM.Common;
using LM.Model.CommonModel;
using LM.Model.Models.MyLMSDB;
using LM.Model.RequestModel;
using LM.Model.ResponseModel;
using LM.Model.SpDbContext;
using LM.Services.UnitOfWork;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LM.Services.Repositories.Implementation;

public class RequestBookRepository : IRequestBookRepository
{
    private readonly LMSDbContext _context;
    private readonly ILogger<RequestBookRepository> _logger;
    private readonly LMSSpContext _spContext;
    private readonly IUnitOfWork _unitOfWork;

    public RequestBookRepository(
        LMSDbContext context,
        IUnitOfWork unitOfWork,
        ILogger<RequestBookRepository> logger,
        LMSSpContext spContext)
    {
        _context = context;
        _logger = logger;
        _spContext = spContext;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Page> List(Dictionary<string, object> parameters)
    {
        _logger.LogInformation("Fetching list of RequestedBook with parameters: {@Params}", parameters);

        try
        {
            var xmlParam = CommonHelper.DictionaryToXml(parameters, "Search");
            string sqlQuery = "SP_GetAllRequestBook {0}";
            object[] param = { xmlParam };

            var res = await _spContext.ExecutreStoreProcedureResultList(sqlQuery, param);

            var list = JsonConvert.DeserializeObject<List<LMSRequestBookResponseModel>>(res.Result?.ToString() ?? "[]");
            

            if (list == null || !list.Any())
            {
                _logger.LogWarning("No Requested Book found with the given parameters: {@Params}", parameters);
                throw new HttpStatusCodeException(404, "No Requested Book found");
            }

            _logger.LogInformation("Requested Book list retrieved successfully.");
            return res;
        }
        catch (HttpStatusCodeException ex)
        {
            _logger.LogWarning("Known error occurred while fetching Requested Book list: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching Requested Book list with parameters: {@Params}", parameters);
            throw new HttpStatusCodeException(500, "An unexpected error occurred while fetching the Requested Book list");
        }
    }
    
    
    //insert book request
       public async Task<List<LMSRequestBookResponseModel>> InsertBookRequest(string UserSid,string BookSid,List<LMSRequestBookRequestModel> Requestbooks)
{
    _logger.LogInformation("Inserting {Count} new book Request", Requestbooks?.Count ?? 0);
    
    try
    {
        if (Requestbooks == null || Requestbooks.Count == 0)
        {
            _logger.LogWarning("Insert failed: Request Book list cannot be empty.");
            throw new HttpStatusCodeException(400, "Request Book list cannot be empty.");
        }

        List<RequestBook> requestbookList = new List<RequestBook>();

        foreach (var requestbook in Requestbooks)
        {
            var user = await _unitOfWork.GetRepository<User>()
                .SingleOrDefaultAsync(u => u.UserSid == UserSid);
            
            var book = await _unitOfWork.GetRepository<Book>()
                .SingleOrDefaultAsync(b => b.BookSid == BookSid);
            
            var rb = new RequestBook
            {
                RequestBookSid = "REQ" + Guid.NewGuid(),
                UserId = user.UserId,
                BookId = book.BookId,
                CreatedBy = user.UserId,
                ModifiedBy = user.UserId,
            };

            requestbookList.Add(rb);
        }

        await _unitOfWork.GetRepository<RequestBook>().InsertAsync(requestbookList);
        await _unitOfWork.CommitAsync();

        var resrequestBooks = requestbookList.Select(rb => new LMSRequestBookResponseModel()
        {
            RequestBookSid = rb.RequestBookSid,
            RequestDate = DateTime.Now,
            RequestBookStatus = (int)Enums.RequestDone,
            CreatedAt =  DateTime.Now,
            ModifiedAt = DateTime.Now,
        }).ToList();

        _logger.LogInformation("Successfully inserted {Count} book request", resrequestBooks.Count);
        return resrequestBooks;
    }
    catch (HttpStatusCodeException ex)
    {
        _logger.LogWarning("Known error occurred while inserting book request: {Message}", ex.Message);
        throw;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error occurred while inserting book request");
        throw new HttpStatusCodeException(500, "An unexpected error occurred while inserting book request");
    }
}
}