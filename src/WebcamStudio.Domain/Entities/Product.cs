using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;


public class Product : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
}
