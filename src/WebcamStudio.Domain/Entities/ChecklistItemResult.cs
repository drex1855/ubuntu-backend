using WebcamStudio.Domain.Common;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Domain.Entities;


public class ChecklistItemResult : AuditableEntity
{
    public Guid ChecklistRunId { get; set; }
    public ChecklistRun ChecklistRun { get; set; } = null!;

    public Guid TemplateItemId { get; set; }
    public ChecklistTemplateItem TemplateItem { get; set; } = null!;

    public ChecklistItemStatus Status { get; set; }
    public string? Observation { get; set; }

    
    public string? AttachmentFileName { get; set; }
    public string? AttachmentContentType { get; set; }
}
