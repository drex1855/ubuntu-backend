using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;

/// <summary>
/// Persona que escribio al WhatsApp del estudio, normalmente a traves del formulario
/// publico del sitio (Source = "SitioWeb"). No esta ligada a una ModelAccount -- es
/// gente externa (aspirantes, clientes) que el estudio quiere poder etiquetar y, si dejo
/// correo o telefono, usar para marketing (correos masivos, exportar numeros).
/// </summary>
public class Contact : AuditableEntity
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Notes { get; set; }

    /// <summary>De donde vino el contacto (hoy siempre "SitioWeb"; queda como string
    /// simple para no tener que migrar un enum si mas adelante se suman otros canales).</summary>
    public string Source { get; set; } = "SitioWeb";

    public bool ConsentGiven { get; set; }
    public DateTime ConsentGivenAt { get; set; }

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
