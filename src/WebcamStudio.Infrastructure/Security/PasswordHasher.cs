using Microsoft.AspNetCore.Identity;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<ModelAccount> _identityHasher = new();

    public string Hash(string password) =>
        _identityHasher.HashPassword(default!, password);

    public bool Verify(string password, string passwordHash)
    {
        var result = _identityHasher.VerifyHashedPassword(default!, passwordHash, password);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
