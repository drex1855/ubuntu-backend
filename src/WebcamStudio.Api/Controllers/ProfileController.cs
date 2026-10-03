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

    [Authorize(Roles = nameof(AccountRole.Admin))]
    [EnableRateLimiting("SensitiveAccountAction")]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct) =>
        HandleResult(await _service.ChangePasswordAsync(CurrentAccountId, request, ct));
}
