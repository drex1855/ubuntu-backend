using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.Interfaces.Services;


public interface ICurrentUserService
{
    Guid? AccountId { get; }
    string? Email { get; }
    AccountRole? Role { get; }
    bool IsAuthenticated { get; }
}
