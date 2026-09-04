using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebcamStudio.Application.Audit;
using WebcamStudio.Application.Common;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Api.Controllers;

/// <summary>Consulta del registro de auditoria transversal (nodo Z del diagrama). Solo Admin.</summary>
[Authorize(Roles = nameof(AccountRole.Admin))]
public class AuditController : ApiControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    private const int DefaultTake = 100;
    private const int MaxTake = 200;

    [HttpGet]
    public async Task<IActionResult> GetRecent([FromQuery] string? module, [FromQuery] int take, CancellationToken ct)
    {
        var effectiveTake = take <= 0 ? DefaultTake : Math.Min(take, MaxTake);
        var logs = await _auditService.GetRecentAsync(module, effectiveTake, ct);
        var dtos = logs.Select(l => new AuditLogDto(
            l.Id, l.AccountId, l.Module, l.Action, l.EntityName, l.EntityId, l.DetailsJson, l.CreatedAt)).ToList();

        return Ok(ApiResponse<List<AuditLogDto>>.Ok(dtos));
    }
}
