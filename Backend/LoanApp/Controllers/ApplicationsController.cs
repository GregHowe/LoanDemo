using LoanApp.DTOs;
using LoanApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly LoanService _loanService;

    public ApplicationsController(LoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpPost]
    public async Task<IActionResult> PostApplication(ApplicationRequest request)
    {
        var result = await _loanService.ProcessApplicationAsync(request);

        if (!result.Approved)
            return BadRequest(result);

        return Ok(result);
    }

}
