using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.Interfaces.Services;

/// <summary>
/// Da acceso al usuario autenticado de la solicitud HTTP actual, sin que la capa
/// de Application tenga que conocer HttpContext (eso lo implementa Infrastructure/Api
/// leyendo los claims del JWT).
/// </summary>
public interface ICurrentUserService
{
    Guid? AccountId { get; }
    string? Email { get; }
    AccountRole? Role { get; }
    bool IsAuthenticated { get; }
}
