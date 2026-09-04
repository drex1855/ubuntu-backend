using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;

/// <summary>
/// Movimiento de horas de una cuenta de modelo, cargado por el staff (Admin/Monitor).
/// El signo de Minutes indica la operacion: positivo = sumo, negativo = resto -- mismo
/// formato que ya usaba la calculadora del lado del cliente que este modulo reemplaza.
/// La modelo afectada solo puede consultar las suyas, nunca cargarlas ni modificarlas
/// (ver HourEntriesController.GetMine).
/// </summary>
public class HourEntry : AuditableEntity
{
    public Guid ModelAccountId { get; set; }
    public ModelAccount ModelAccount { get; set; } = null!;

    public int Minutes { get; set; }
    public string? Note { get; set; }

    public Guid RegisteredByAccountId { get; set; }
    public DateTime RegisteredAt { get; set; }
}
