using WebcamStudio.Domain.Common;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Domain.Entities;

/// <summary>
/// Multa aplicada por el estudio a una cuenta de modelo. Solo el staff (Admin/Monitor)
/// la crea y cambia su estado; la modelo afectada solo puede verla en su perfil
/// (ver FinesController/GetMyFinesAsync).
/// </summary>
public class Fine : AuditableEntity
{
    public Guid ModelAccountId { get; set; }
    public ModelAccount ModelAccount { get; set; } = null!;

    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;

    public FineStatus Status { get; set; } = FineStatus.PendientePorCobrar;
    public DateTime IssuedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
