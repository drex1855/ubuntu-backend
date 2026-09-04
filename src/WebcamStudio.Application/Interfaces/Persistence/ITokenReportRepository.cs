using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Interfaces.Persistence;

public interface ITokenReportRepository : IRepository<TokenReport>
{
    /// <summary>
    /// Trae reportes filtrados opcionalmente por modelo, sitio y rango de periodo.
    /// Base para el resumen del nodo "Generar resumen por modelo y sitio" (E6):
    /// el agrupamiento/suma se hace en el servicio de Application, no aqui, para que
    /// la logica de negocio (como se calcula el resumen) no quede escondida en el repositorio.
    /// </summary>
    Task<List<TokenReport>> SearchAsync(
        Guid? modelAccountId,
        Guid? siteId,
        DateOnly? periodFrom,
        DateOnly? periodTo,
        CancellationToken ct = default);
}
