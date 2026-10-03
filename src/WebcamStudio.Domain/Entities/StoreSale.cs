using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;

public class StoreSale : AuditableEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid ModelAccountId { get; set; }
    public ModelAccount ModelAccount { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalAmount { get; set; }
    public bool IsCredit { get; set; }

    public Guid SoldByAccountId { get; set; }
    public DateTime SoldAt { get; set; }
}
