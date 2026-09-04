using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;

/// <summary>
/// Sitio o plataforma externa donde una modelo transmite y genera tokens
/// (ej. Chaturbate, Stripchat, etc.). Corresponde al nodo "Seleccionar sitio o cuenta" (E2)
/// del modulo "Reporte de tokens" en el diagrama. Se modela como catalogo propio
/// (en vez de un string libre) para poder agregar sitios nuevos sin tocar codigo
/// y para poder agrupar reportes por sitio de forma confiable.
/// </summary>
public class Site : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>Valor en USD de un solo token en este sitio (ej. 0.05). Se usa para calcular
    /// automaticamente TokenReport.MonetaryValue a partir de TokensAmount -- ver
    /// TokenReportService.CreateAsync. Cada sitio puede tener su propia tasa.</summary>
    public decimal TokenValueUsd { get; set; }

    public ICollection<TokenReport> TokenReports { get; set; } = new List<TokenReport>();
}
