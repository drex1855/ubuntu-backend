using WebcamStudio.Domain.Common;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Domain.Entities;

/// <summary>
/// Cuenta de una modelo (o de un administrador) dentro del sistema.
/// Es la entidad central: de aqui cuelgan los reportes de tokens y los movimientos de
/// inventario/checklist que registra cada usuario.
/// Modulo del diagrama: "Cuentas de modelos" (G) + raiz de "Validar identidad y permisos" (C4).
/// </summary>
public class ModelAccount : AuditableEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public AccountRole Role { get; set; } = AccountRole.Modelo;
    public AccountStatus Status { get; set; } = AccountStatus.Activo;
    public AccountGender? Gender { get; set; }

    /// <summary>Intentos de login fallidos consecutivos. Se resetea a 0 en cada login exitoso.</summary>
    public int FailedLoginAttempts { get; set; }

    /// <summary>Si tiene un valor futuro, la cuenta esta temporalmente bloqueada por
    /// demasiados intentos fallidos (ver AuthService.LoginAsync).</summary>
    public DateTime? LockedUntil { get; set; }

    public ICollection<TokenReport> TokenReports { get; set; } = new List<TokenReport>();
    public ICollection<ChecklistRun> ChecklistRuns { get; set; } = new List<ChecklistRun>();

    /// <summary>
    /// Habilita el acceso y las operaciones de la cuenta.
    /// Corresponde al nodo "Habilitar acceso y operaciones" (G4) del diagrama.
    /// </summary>
    public void Activate() => Status = AccountStatus.Activo;

    /// <summary>
    /// Bloquea el acceso y las operaciones de la cuenta.
    /// Corresponde al nodo "Bloquear acceso y operaciones" (G5) del diagrama.
    /// </summary>
    public void Deactivate() => Status = AccountStatus.Desactivado;

    public bool IsActive => Status == AccountStatus.Activo;
}
