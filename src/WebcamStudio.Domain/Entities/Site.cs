using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;

public class Site : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public decimal TokenValueUsd { get; set; }

    public ICollection<TokenReport> TokenReports { get; set; } = new List<TokenReport>();
}
