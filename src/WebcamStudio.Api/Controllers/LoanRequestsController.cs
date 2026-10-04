using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebcamStudio.Application.Common;
using WebcamStudio.Application.LoanRequests;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Api.Controllers;


[Authorize]
public class LoanRequestsController : ApiControllerBase
{
    private readonly ILoanRequestService _service;

    public LoanRequestsController(ILoanRequestService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLoanRequestRequest request, CancellationToken ct) =>
        HandleResult(await _service.CreateAsync(CurrentAccountId, request, ct));

    [Authorize(Roles = $"{nameof(AccountRole.Admin)},{nameof(AccountRole.Monitor)}")]
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] LoanRequestStatus? status, CancellationToken ct)
    {
        var requests = await _service.SearchAsync(status, ct);
        return Ok(ApiResponse<List<LoanRequestDto>>.Ok(requests));
    }

    [Authorize(Roles = $"{nameof(AccountRole.Admin)},{nameof(AccountRole.Monitor)}")]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> SetStatus(
        Guid id, [FromBody] UpdateLoanRequestStatusRequest request, CancellationToken ct) =>
        HandleResult(await _service.SetStatusAsync(id, request, ct));
}
