using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Interfaces.Services;

public interface IJwtTokenGenerator
{
    /// <summary>Genera un JWT firmado para la cuenta dada. El token incluye el id
    /// de cuenta y el rol como claims, que es lo que luego lee ICurrentUserService.</summary>
    (string Token, DateTime ExpiresAt) GenerateToken(ModelAccount account);
}
