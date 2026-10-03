using WebcamStudio.Domain.Common;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Domain.Entities;


public class MaintenanceRequest : AuditableEntity
{
    public Guid ChecklistRunId { get; set; }
    public ChecklistRun ChecklistRun { get; set; } = null!;

    public Guid? ChecklistItemResultId { get; set; }
    public ChecklistItemResult? ChecklistItemResult { get; set; }

    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public string Description { get; set; } = string.Empty;
    public MaintenanceRequestStatus Status { get; set; } = MaintenanceRequestStatus.Pendiente;
    public DateTime? ResolvedAt { get; set; }
}
