using MockExternalService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MockExternalService.Controllers;

[ApiController]
[Route("mock/applications")]
public class MockApplicationsController : ControllerBase
{
    private static readonly Dictionary<string, ApplicationRequest> _store = new();

    [HttpGet("{ssn}")]
    public IActionResult GetBySsn(string ssn)
    {
        if (_store.ContainsKey(ssn))
            return Ok(_store[ssn]);
        return NotFound();
    }

    [HttpPost]
    public IActionResult Create([FromBody] ApplicationRequest request)
    {
        if (_store.ContainsKey(request.SSN))
            return Conflict(new { message = "Customer already exists" });

        _store[request.SSN] = request;
        return Ok(new { message = "Customer created" });
    }

    [HttpPut("{ssn}")]
    public IActionResult Update(string ssn, [FromBody] ApplicationRequest request)
    {
        if (!_store.ContainsKey(ssn))
            return NotFound(new { message = "Customer not found" });

        _store[ssn] = request;
        return Ok(new { message = "Customer updated" });
    }
}
