using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;

/// <summary>
/// Una ejecucion completa del checklist de una habitacion en una fecha dada.
/// Agrupa los resultados de cada ChecklistTemplateItem revisado. Corresponde al
/// nodo "Guardar checklist" (F10): todo el proceso F1-F9 termina persistiendo un ChecklistRun.
/// </summary>
public class ChecklistRun : AuditableEntity
{
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public Guid PerformedByAccountId { get; set; }
    public ModelAccount PerformedByAccount { get; set; } = null!;

    public DateTime PerformedAt { get; set; }

    /// <summary>
    /// Materiales disponibles registrados en la revision (nodo "Registrar materiales
    /// disponibles", F9). Texto libre por ahora; si el negocio necesita cantidades
    /// estructuradas por material, esto se puede volver una tabla propia mas adelante
    /// sin afectar el resto del checklist.
    /// </summary>
    public string? AvailableMaterialsNotes { get; set; }

    /// <summary>Foto opcional adjunta a las notas de materiales (ver IFileStorageService).</summary>
    public string? MaterialsAttachmentFileName { get; set; }
    public string? MaterialsAttachmentContentType { get; set; }

    public ICollection<ChecklistItemResult> ItemResults { get; set; } = new List<ChecklistItemResult>();
    public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
}
