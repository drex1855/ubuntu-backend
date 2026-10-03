using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebcamStudio.Application.Common;
using WebcamStudio.Application.TokenReports;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Api.Controllers;

[Authorize]
public class TokenReportsController : ApiControllerBase
{
    private readonly ITokenReportService _service;

    public TokenReportsController(ITokenReportService service)
    {
        _service = service;
    }

    private const string StaffRoles = $"{nameof(AccountRole.Admin)},{nameof(AccountRole.Monitor)}";

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTokenReportRequest request, CancellationToken ct)
    {
       
        if (!User.IsInRole(nameof(AccountRole.Admin)) && !User.IsInRole(nameof(AccountRole.Monitor))
            && request.ModelAccountId != CurrentAccountId)
            return Forbid();

        return HandleResult(await _service.CreateAsync(CurrentAccountId, request, ct));
    }

   
    [Authorize(Roles = StaffRoles)]
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] Guid? modelAccountId, [FromQuery] Guid? siteId,
        [FromQuery] DateOnly? periodFrom, [FromQuery] DateOnly? periodTo, CancellationToken ct)
    {
        var reports = await _service.SearchAsync(new TokenReportSearchRequest(modelAccountId, siteId, periodFrom, periodTo), ct);
        return Ok(ApiResponse<List<TokenReportDto>>.Ok(reports));
    }

    /// <summary>Resumen agregado de todas las modelos: solo staff (ver GetMineSummary para el self-service).</summary>
    [Authorize(Roles = StaffRoles)]
    [HttpGet("summary")]
    public async Task<IActionResult> Summary(
        [FromQuery] Guid? modelAccountId, [FromQuery] Guid? siteId,
        [FromQuery] DateOnly? periodFrom, [FromQuery] DateOnly? periodTo, CancellationToken ct)
    {
        var summary = await _service.GetSummaryAsync(new TokenReportSearchRequest(modelAccountId, siteId, periodFrom, periodTo), ct);
        return Ok(ApiResponse<List<TokenSummaryDto>>.Ok(summary));
    }

    /// <summary>Mis propios reportes de tokens (cualquier rol autenticado, siempre acotado a si mismo).</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMine(
        [FromQuery] Guid? siteId, [FromQuery] DateOnly? periodFrom, [FromQuery] DateOnly? periodTo, CancellationToken ct)
    {
        var reports = await _service.SearchAsync(new TokenReportSearchRequest(CurrentAccountId, siteId, periodFrom, periodTo), ct);
        return Ok(ApiResponse<List<TokenReportDto>>.Ok(reports));
    }

    /// <summary>Mi propio resumen agregado (cualquier rol autenticado, siempre acotado a si mismo).</summary>
    [HttpGet("me/summary")]
    public async Task<IActionResult> GetMineSummary(
        [FromQuery] Guid? siteId, [FromQuery] DateOnly? periodFrom, [FromQuery] DateOnly? periodTo, CancellationToken ct)
    {
        var summary = await _service.GetSummaryAsync(new TokenReportSearchRequest(CurrentAccountId, siteId, periodFrom, periodTo), ct);
        return Ok(ApiResponse<List<TokenSummaryDto>>.Ok(summary));
    }
}
