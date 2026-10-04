using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;


public class HourEntry : AuditableEntity
{
    public Guid ModelAccountId { get; set; }
    public ModelAccount ModelAccount { get; set; } = null!;

    public int Minutes { get; set; }
    public string? Note { get; set; }

    public Guid RegisteredByAccountId { get; set; }
    public DateTime RegisteredAt { get; set; }
}
