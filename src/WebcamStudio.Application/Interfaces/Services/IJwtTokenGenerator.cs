using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Interfaces.Services;

public interface IJwtTokenGenerator
{
    
    (string Token, DateTime ExpiresAt) GenerateToken(ModelAccount account);
}
