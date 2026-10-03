using WebcamStudio.Domain.Common;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Domain.Entities;

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
