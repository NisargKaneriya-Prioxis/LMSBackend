using EvaluationAPI.Controllers;
using LM.Services.Repositories.Interface;

namespace LMS.Controllers;

public class ReturnBookController : BaseController
{
    private readonly IReturnBookRepository _returnbookRepository;
    private readonly ILogger<ReturnBookController> _logger;

    public ReturnBookController(IReturnBookRepository ReturnBookRepository, ILogger<ReturnBookController> logger)
    {
        _returnbookRepository = ReturnBookRepository;
        _logger = logger;
    }
    
}