using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;

/// <summary>
/// Reporte periodico de tokens generados por una modelo en un sitio especifico.
/// Corresponde a los nodos E1-E5 del diagrama: "Seleccionar modelo" -> "Seleccionar sitio
/// o cuenta" -> "Registrar tokens" -> "Validar periodo y monto" -> "Guardar reporte".
/// El resumen por modelo/sitio (E6) se calcula a partir de estos registros, no se
/// persiste aparte, para no tener datos derivados que se puedan desincronizar.
/// </summary>
public class TokenReport : AuditableEntity
{
    public Guid ModelAccountId { get; set; }
    public ModelAccount ModelAccount { get; set; } = null!;

    public Guid SiteId { get; set; }
    public Site Site { get; set; } = null!;

    /// <summary>
    /// Primer dia del periodo que cubre el reporte (se normaliza siempre al dia 1
    /// del mes para poder agrupar por periodo facilmente en el resumen).
    /// </summary>
    public DateOnly Period { get; set; }

    public decimal TokensAmount { get; set; }

    /// <summary>
    /// Valor monetario equivalente de los tokens, si aplica (puede quedar en 0
    /// si el negocio todavia no define la tasa de conversion para ese sitio).
    /// </summary>
    public decimal MonetaryValue { get; set; }

    public Guid RegisteredByAccountId { get; set; }
    public DateTime RegisteredAt { get; set; }
}
