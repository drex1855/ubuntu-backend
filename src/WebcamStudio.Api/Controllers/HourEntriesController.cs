using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebcamStudio.Application.Common;
using WebcamStudio.Application.HourEntries;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Api.Controllers;

/// <summary>Modulo "Horas": el staff anota los movimientos de horas de cada modelo;
/// cada cuenta consulta solo las suyas (ver GetMine).</summary>
[Authorize]
public class HourEntriesController : ApiControllerBase
{
    private readonly IHourEntryService _service;

    public HourEntriesController(IHourEntryService service)
    {
        _service = service;
    }

    private const string StaffRoles = $"{nameof(AccountRole.Admin)},{nameof(AccountRole.Monitor)}";

    [Authorize(Roles = StaffRoles)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHourEntryRequest request, CancellationToken ct) =>
        HandleResult(await _service.CreateAsync(CurrentAccountId, request, ct));

    [Authorize(Roles = StaffRoles)]
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] Guid? modelAccountId, CancellationToken ct)
    {
        var entries = await _service.SearchAsync(modelAccountId, ct);
        return Ok(ApiResponse<List<HourEntryDto>>.Ok(entries));
    }

    /// <summary>Mis propias horas, solo lectura -- cualquier cuenta autenticada consulta las suyas.</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMine(CancellationToken ct)
    {
        var entries = await _service.GetMineAsync(CurrentAccountId, ct);
        return Ok(ApiResponse<List<HourEntryDto>>.Ok(entries));
    }
}
