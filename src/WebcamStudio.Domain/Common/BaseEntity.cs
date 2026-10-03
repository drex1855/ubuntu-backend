namespace WebcamStudio.Domain.Common;


public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}


public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedByAccountId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedByAccountId { get; set; }
}
