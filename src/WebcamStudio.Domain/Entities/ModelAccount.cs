using WebcamStudio.Domain.Common;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Domain.Entities;

public class ModelAccount : AuditableEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public AccountRole Role { get; set; } = AccountRole.Modelo;
    public AccountStatus Status { get; set; } = AccountStatus.Activo;
    public AccountGender? Gender { get; set; }

   
    public int FailedLoginAttempts { get; set; }

 
    public DateTime? LockedUntil { get; set; }

    public ICollection<TokenReport> TokenReports { get; set; } = new List<TokenReport>();
    public ICollection<ChecklistRun> ChecklistRuns { get; set; } = new List<ChecklistRun>();

   
    public void Activate() => Status = AccountStatus.Activo;

    
    public void Deactivate() => Status = AccountStatus.Desactivado;

    public bool IsActive => Status == AccountStatus.Activo;
}
