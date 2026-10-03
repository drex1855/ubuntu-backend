using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;


public class AuditLog : BaseEntity
{
    public Guid? AccountId { get; set; }
    public ModelAccount? Account { get; set; }

    
    public string Module { get; set; } = string.Empty;

    
    public string Action { get; set; } = string.Empty;

    public string? EntityName { get; set; }
    public Guid? EntityId { get; set; }

    
    public string? DetailsJson { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
