using EvaluationAPI.Controllers;
using LM.Model.RequestModel;
using LM.Model.ResponseModel;
using LM.Services.Repositories.Implementation;
using LM.Services.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [Authorize(Roles = "Admin")]
    [HttpPost("InsertReturnbook")]
    public async Task<ActionResult<List<LMSBorrowedBookResponseModel>>> ReturnBook(
    [FromQuery] string borrowedSid,
    [FromQuery] string userSid)
    {
        var createdBook = await _returnbookRepository.ReturnBook(borrowedSid, userSid);

        if (createdBook == null)
        {
            _logger.LogWarning("Return book failed. borrowedSid={BorrowedSid}, userSid={UserSid}", borrowedSid, userSid);
            return BadRequest("Return operation failed.");
        }

        _logger.LogInformation("Book returned successfully for borrowedSid={BorrowedSid}", borrowedSid);
        return Ok(createdBook);
    }

}