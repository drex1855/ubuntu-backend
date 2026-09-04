using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebcamStudio.Application.Common;
using WebcamStudio.Application.Fines;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Api.Controllers;

/// <summary>Modulo "Multas": el staff aplica y controla multas a cuentas de modelo.</summary>
[Authorize]
public class FinesController : ApiControllerBase
{
    private readonly IFineService _service;

    public FinesController(IFineService service)
    {
        _service = service;
    }

    private const string StaffRoles = $"{nameof(AccountRole.Admin)},{nameof(AccountRole.Monitor)}";

    [Authorize(Roles = StaffRoles)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFineRequest request, CancellationToken ct) =>
        HandleResult(await _service.CreateAsync(request, ct));

    [Authorize(Roles = StaffRoles)]
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] Guid? modelAccountId, [FromQuery] FineStatus? status, CancellationToken ct)
    {
        var fines = await _service.SearchAsync(modelAccountId, status, ct);
        return Ok(ApiResponse<List<FineDto>>.Ok(fines));
    }

    [Authorize(Roles = StaffRoles)]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] UpdateFineStatusRequest request, CancellationToken ct) =>
        HandleResult(await _service.SetStatusAsync(id, request, ct));

    /// <summary>Multas propias, solo lectura -- cualquier cuenta autenticada consulta las suyas.</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMyFines(CancellationToken ct)
    {
        var fines = await _service.GetMyFinesAsync(CurrentAccountId, ct);
        return Ok(ApiResponse<List<FineDto>>.Ok(fines));
    }
}
