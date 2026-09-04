using WebcamStudio.Domain.Common;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Domain.Entities;

/// <summary>
/// Solicitud de prestamo hecha por una modelo o un monitor. Al crearse dispara un
/// correo al dueno del estudio (ver IEmailSender) y queda pendiente hasta que un Admin
/// la apruebe o la rechace.
/// </summary>
public class LoanRequest : AuditableEntity
{
    public Guid RequestedByAccountId { get; set; }
    public ModelAccount RequestedByAccount { get; set; } = null!;

    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;

    public LoanRequestStatus Status { get; set; } = LoanRequestStatus.Pendiente;
    public DateTime RequestedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
