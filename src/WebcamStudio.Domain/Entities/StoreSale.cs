using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;

/// <summary>
/// Registro de que se le entrego un producto de la tienda a una modelo: lo pago en el
/// momento (IsCredit = false, no afecta su cuenta) o lo tomo a credito (IsCredit = true,
/// se suma a su cuenta -- ver InventoryService.GetDebtsAsync). StoreDebtPayment es la
/// forma de bajar ese total cuando la modelo se pone al dia.
/// </summary>
public class StoreSale : AuditableEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid ModelAccountId { get; set; }
    public ModelAccount ModelAccount { get; set; } = null!;

    public int Quantity { get; set; }

    /// <summary>Precio unitario al momento de la venta (se copia de Product.Price para que
    /// un cambio de precio despues no altere ventas ya registradas).</summary>
    public decimal UnitPrice { get; set; }

    public decimal TotalAmount { get; set; }
    public bool IsCredit { get; set; }

    public Guid SoldByAccountId { get; set; }
    public DateTime SoldAt { get; set; }
}
