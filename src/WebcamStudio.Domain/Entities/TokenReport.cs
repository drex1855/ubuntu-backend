using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;

public class TokenReport : AuditableEntity
{
    public Guid ModelAccountId { get; set; }
    public ModelAccount ModelAccount { get; set; } = null!;

    public Guid SiteId { get; set; }
    public Site Site { get; set; } = null!;

    public DateOnly Period { get; set; }

    public decimal TokensAmount { get; set; }

    public decimal MonetaryValue { get; set; }

    public Guid RegisteredByAccountId { get; set; }
    public DateTime RegisteredAt { get; set; }
}
