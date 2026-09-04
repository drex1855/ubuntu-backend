using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.Common;

/// <summary>
/// Representa al usuario autenticado en la solicitud actual, extraido del JWT.
/// Se usa en los servicios para saber "quien" esta realizando la accion sin acoplar
/// la capa de Application a HttpContext (eso vive en Infrastructure, ver ICurrentUserService).
/// </summary>
public class CurrentUser
{
    public Guid AccountId { get; set; }
    public string Email { get; set; } = string.Empty;
    public AccountRole Role { get; set; }
}
