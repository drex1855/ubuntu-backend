using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;

public class StoreDebtPayment : AuditableEntity
{
    public Guid ModelAccountId { get; set; }
    public ModelAccount ModelAccount { get; set; } = null!;

    public decimal Amount { get; set; }
    public Guid RegisteredByAccountId { get; set; }
    public DateTime PaidAt { get; set; }
}
