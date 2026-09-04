using WebcamStudio.Domain.Common;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Domain.Entities;

/// <summary>
/// Resultado de revisar un ChecklistTemplateItem especifico dentro de un ChecklistRun.
/// Corresponde a la decision "Elemento en buen estado" (F4) y a los nodos
/// "Marcar como bueno" (F5) / "Marcar como malo" + "Registrar observacion" (F6-F7).
/// </summary>
public class ChecklistItemResult : AuditableEntity
{
    public Guid ChecklistRunId { get; set; }
    public ChecklistRun ChecklistRun { get; set; } = null!;

    public Guid TemplateItemId { get; set; }
    public ChecklistTemplateItem TemplateItem { get; set; } = null!;

    public ChecklistItemStatus Status { get; set; }
    public string? Observation { get; set; }

    /// <summary>Nombre del archivo guardado (GUID + extension) de una foto opcional
    /// adjunta a este item -- ver IFileStorageService. Null si no se adjunto nada.</summary>
    public string? AttachmentFileName { get; set; }
    public string? AttachmentContentType { get; set; }
}
