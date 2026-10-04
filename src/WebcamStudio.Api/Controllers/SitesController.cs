using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebcamStudio.Application.Common;
using WebcamStudio.Application.Sites;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Api.Controllers;


[Authorize]
public class SitesController : ApiControllerBase
{
    private readonly ISiteService _service;

    public SitesController(ISiteService service)
    {
        _service = service;
    }

    private const string StaffRoles = $"{nameof(AccountRole.Admin)},{nameof(AccountRole.Monitor)}";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var sites = await _service.GetAllAsync(ct);
        return Ok(ApiResponse<List<SiteDto>>.Ok(sites));
    }

    [Authorize(Roles = StaffRoles)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSiteRequest request, CancellationToken ct) =>
        HandleResult(await _service.CreateAsync(request, ct));

    [Authorize(Roles = StaffRoles)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSiteRequest request, CancellationToken ct) =>
        HandleResult(await _service.UpdateAsync(id, request, ct));
}
