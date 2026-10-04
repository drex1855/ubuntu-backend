using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;

public class ChecklistTemplateItem : AuditableEntity
{
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }

    public ICollection<ChecklistItemResult> Results { get; set; } = new List<ChecklistItemResult>();
}
