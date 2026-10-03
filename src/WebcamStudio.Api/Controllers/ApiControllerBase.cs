using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebcamStudio.Application.Common;

namespace WebcamStudio.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.Succeeded)
            return Ok(ApiResponse<T>.Ok(result.Value!));

        return BadRequest(ApiResponse<T>.Fail(result.Error ?? "Solicitud invalida."));
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.Succeeded)
            return Ok(ApiResponse<object>.Ok(new { }));

        return BadRequest(ApiResponse<object>.Fail(result.Error ?? "Solicitud invalida."));
    }

    
    protected Guid CurrentAccountId =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : throw new InvalidOperationException("No hay una cuenta autenticada en la solicitud.");
}
