using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;

/// <summary>
/// Etiqueta libre que el staff crea para clasificar Contacts (ej. "Se le puede escribir",
/// "Ya contactada", "Interesada"). Es de creacion libre, no un enum fijo.
/// </summary>
public class Tag : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
