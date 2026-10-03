using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;


public class ChecklistRun : AuditableEntity
{
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public Guid PerformedByAccountId { get; set; }
    public ModelAccount PerformedByAccount { get; set; } = null!;

    public DateTime PerformedAt { get; set; }

    
    public string? AvailableMaterialsNotes { get; set; }

    
    public string? MaterialsAttachmentFileName { get; set; }
    public string? MaterialsAttachmentContentType { get; set; }

    public ICollection<ChecklistItemResult> ItemResults { get; set; } = new List<ChecklistItemResult>();
    public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
}
