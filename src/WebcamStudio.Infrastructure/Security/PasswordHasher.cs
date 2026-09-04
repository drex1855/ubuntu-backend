using Microsoft.AspNetCore.Identity;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Security;

/// <summary>
/// Usa PasswordHasher&lt;T&gt; de ASP.NET Core Identity (PBKDF2 con HMAC-SHA256, salteado
/// automaticamente), que ya viene incluido en el shared framework de ASP.NET Core.
/// Se eligio en vez de una libreria de terceros como BCrypt.Net para no agregar una
/// dependencia externa extra solo para esto -- es el mismo algoritmo que usa ASP.NET
/// Core Identity "de verdad" cuando se usa con su sistema de usuarios completo.
/// </summary>
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
