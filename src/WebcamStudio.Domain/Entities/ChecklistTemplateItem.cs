using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;

/// <summary>
/// Elemento fijo que debe revisarse en una habitacion (ej. "Camara", "Iluminacion",
/// "Sabanas"). Es la plantilla que se "carga" en el nodo "Cargar checklist" (F2)
/// del diagrama, antes de registrar el resultado de cada revision.
/// </summary>
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
