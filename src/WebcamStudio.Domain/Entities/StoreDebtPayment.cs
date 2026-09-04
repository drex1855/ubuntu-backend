using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;

/// <summary>
/// Abono de una modelo contra su deuda acumulada en la tienda. La deuda total nunca se
/// guarda como un campo mutable: siempre se calcula como
/// suma(StoreSale.TotalAmount) - suma(StoreDebtPayment.Amount), para no perder
/// trazabilidad de cada movimiento.
/// </summary>
public class StoreDebtPayment : AuditableEntity
{
    public Guid ModelAccountId { get; set; }
    public ModelAccount ModelAccount { get; set; } = null!;

    public decimal Amount { get; set; }
    public Guid RegisteredByAccountId { get; set; }
    public DateTime PaidAt { get; set; }
}
