using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Infrastructure.Security;

/// <summary>
/// Lee los claims del JWT ya validado por el middleware de autenticacion de ASP.NET Core.
/// Es la unica clase de todo el backend que sabe que existe HttpContext -- por eso vive
/// en Infrastructure y no en Application, que debe poder probarse sin un servidor HTTP real.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public Guid? AccountId
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email => User?.FindFirstValue(ClaimTypes.Email);

    public AccountRole? Role
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.Role);
            return Enum.TryParse<AccountRole>(value, out var role) ? role : null;
        }
    }
}
