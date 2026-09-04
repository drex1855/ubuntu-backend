using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebcamStudio.Application.Common;
using WebcamStudio.Application.ModelAccounts;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Api.Controllers;

[Authorize(Roles = $"{nameof(AccountRole.Admin)},{nameof(AccountRole.Monitor)}")]
public class ModelAccountsController : ApiControllerBase
{
    private readonly IModelAccountService _service;

    public ModelAccountsController(IModelAccountService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var accounts = await _service.GetAllAsync(ct);
        return Ok(ApiResponse<List<ModelAccountDto>>.Ok(accounts));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        HandleResult(await _service.GetByIdAsync(id, ct));

    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateModelAccountRequest request, CancellationToken ct)
    {
        // Un Monitor no puede crear cuentas Admin ni Monitor (escalacion de privilegios):
        // solo un Admin puede otorgar roles de staff. Monitor solo puede crear cuentas Modelo.
        if (request.Role != AccountRole.Modelo && !User.IsInRole(nameof(AccountRole.Admin)))
            return Forbid();

        return HandleResult(await _service.CreateAsync(request, ct));
    }

   
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateModelAccountRequest request, CancellationToken ct) =>
        HandleResult(await _service.UpdateAsync(id, request, ct));

    
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] UpdateAccountStatusRequest request, CancellationToken ct) =>
        HandleResult(await _service.SetStatusAsync(id, request, ct));

    /// <summary>Restablece la contraseña de cualquier cuenta. Solo Admin -- Monitor puede
    /// administrar cuentas Modelo pero no manejar contraseñas de nadie.</summary>
    [Authorize(Roles = nameof(AccountRole.Admin))]
    [HttpPost("{id:guid}/reset-password")]
    public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordRequest request, CancellationToken ct) =>
        HandleResult(await _service.ResetPasswordAsync(id, request, ct));
}
