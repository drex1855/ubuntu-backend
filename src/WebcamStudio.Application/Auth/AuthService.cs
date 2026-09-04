using WebcamStudio.Application.Common;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.Auth;

/// <summary>
/// Implementa la parte de autenticacion de "Validar identidad y permisos" (C4) del
/// diagrama, reutilizada como login general de la API (no solo para el flujo de WhatsApp).
/// </summary>
public class AuthService : IAuthService
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IAuditService _auditService;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _auditService = auditService;
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var account = await _unitOfWork.ModelAccounts.GetByEmailAsync(request.Email, ct);
        if (account is null)
        {
            // No revelamos si el correo existe o no, para no facilitar enumeracion de cuentas.
            await _auditService.LogAsync("Auth", "LoginFallido", "ModelAccount", null,
                new { request.Email }, ct);
            return Result<LoginResponse>.Failure("Credenciales invalidas.");
        }

        if (account.LockedUntil is { } lockedUntil && lockedUntil > DateTime.UtcNow)
        {
            await _auditService.LogAsync("Auth", "LoginRechazadoCuentaBloqueada", "ModelAccount", account.Id, null, ct);
            return Result<LoginResponse>.Failure(
                "Cuenta bloqueada temporalmente por demasiados intentos fallidos. Intenta de nuevo mas tarde.");
        }

        if (!_passwordHasher.Verify(request.Password, account.PasswordHash))
        {
            account.FailedLoginAttempts++;
            if (account.FailedLoginAttempts >= MaxFailedAttempts)
            {
                account.LockedUntil = DateTime.UtcNow.Add(LockoutDuration);
                account.FailedLoginAttempts = 0;
            }

            _unitOfWork.ModelAccounts.Update(account);
            await _unitOfWork.SaveChangesAsync(ct);

            await _auditService.LogAsync("Auth", "LoginFallido", "ModelAccount", account.Id,
                new { request.Email }, ct);
            return Result<LoginResponse>.Failure("Credenciales invalidas.");
        }

        if (account.Status != AccountStatus.Activo)
        {
            // Corresponde a la rama "Responder acceso no autorizado" (C5) del diagrama,
            // generalizada: una cuenta desactivada no puede autenticarse en ningun modulo.
            await _auditService.LogAsync("Auth", "LoginRechazadoCuentaDesactivada", "ModelAccount", account.Id, null, ct);
            return Result<LoginResponse>.Failure("La cuenta esta desactivada. Contacta a un administrador.");
        }

        if (account.FailedLoginAttempts != 0 || account.LockedUntil is not null)
        {
            account.FailedLoginAttempts = 0;
            account.LockedUntil = null;
            _unitOfWork.ModelAccounts.Update(account);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(account);

        await _auditService.LogAsync("Auth", "LoginExitoso", "ModelAccount", account.Id, null, ct);

        return Result<LoginResponse>.Success(
            new LoginResponse(token, expiresAt, account.Id, account.FullName, account.Role));
    }
}
