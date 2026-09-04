namespace WebcamStudio.Application.Interfaces.Services;

/// <summary>
/// Abstrae el algoritmo de hashing de contraseñas. La implementacion real
/// (Infrastructure) usa PasswordHasher&lt;T&gt; de ASP.NET Core Identity (PBKDF2 + HMACSHA256),
/// que ya viene incluido en el shared framework de ASP.NET Core -- no requiere
/// agregar una libreria externa de terceros solo para esto.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}
