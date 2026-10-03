using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebcamStudio.Application.ModelAccounts;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Api.Controllers;

[Authorize]
public class ProfileController : ApiControllerBase
{
    private readonly IModelAccountService _service;

    public ProfileController(IModelAccountService service)
    {
        _service = service;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct) =>
        HandleResult(await _service.GetByIdAsync(CurrentAccountId, ct));

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateModelAccountRequest request, CancellationToken ct) =>
        HandleResult(await _service.UpdateAsync(CurrentAccountId, request, ct));

    /// <summary>Solo Admin puede cambiar su propia contraseña por autoservicio. Monitor y
    /// Modelo ya no pueden (ver ModelAccountsController.ResetPassword para que un Admin
    /// restablezca la contraseña de cualquier cuenta cuando haga falta).</summary>
    [Authorize(Roles = nameof(AccountRole.Admin))]
    [EnableRateLimiting("SensitiveAccountAction")]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct) =>
        HandleResult(await _service.ChangePasswordAsync(CurrentAccountId, request, ct));
}
