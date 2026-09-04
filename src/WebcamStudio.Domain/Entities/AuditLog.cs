using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;

/// <summary>
/// Registro de auditoria transversal. En el diagrama, TODOS los flujos (WhatsApp,
/// Inventario, Tokens, Checklist, Cuentas) convergen en el nodo "Registrar auditoria" (Z)
/// antes de responder al frontend. En el codigo esto se implementa como un servicio
/// unico (IAuditService) que cualquier modulo puede llamar, en vez de que cada modulo
/// reinvente su propio log -- asi un modulo nuevo que se agregue a futuro solo tiene
/// que inyectar IAuditService y ya queda cubierto.
/// </summary>
public class AuditLog : BaseEntity
{
    public Guid? AccountId { get; set; }
    public ModelAccount? Account { get; set; }

    /// <summary>Modulo de origen: "WhatsApp", "Inventario", "Tokens", "Checklist", "Cuentas".</summary>
    public string Module { get; set; } = string.Empty;

    /// <summary>Accion realizada, en texto libre corto: "MensajeProcesado", "MovimientoRegistrado", etc.</summary>
    public string Action { get; set; } = string.Empty;

    public string? EntityName { get; set; }
    public Guid? EntityId { get; set; }

    /// <summary>Detalle adicional serializado en JSON, para no tener que migrar la tabla
    /// cada vez que un modulo quiere auditar un dato distinto.</summary>
    public string? DetailsJson { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
