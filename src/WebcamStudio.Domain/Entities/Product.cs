using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;

/// <summary>
/// Producto de la tienda: solo nombre y precio. No se lleva cantidad/stock -- cada vez
/// que una modelo saca un producto se registra en StoreSale y se acumula a su cuenta.
/// </summary>
public class Product : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
}
