using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;

/// <summary>
/// Habitacion/estudio fisico que se revisa periodicamente. Modulo del diagrama:
/// "Checklist de habitaciones" (F), nodo "Seleccionar habitacion" (F1).
/// </summary>
public class Room : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<ChecklistTemplateItem> TemplateItems { get; set; } = new List<ChecklistTemplateItem>();
    public ICollection<ChecklistRun> ChecklistRuns { get; set; } = new List<ChecklistRun>();
}
