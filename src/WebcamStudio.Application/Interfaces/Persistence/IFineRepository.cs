using WebcamStudio.Domain.Entities;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.Interfaces.Persistence;

public interface IFineRepository : IRepository<Fine>
{
    /// <summary>Multas filtradas opcionalmente por modelo y/o estado, para el historial del staff.</summary>
    Task<List<Fine>> SearchAsync(Guid? modelAccountId, FineStatus? status, CancellationToken ct = default);
}
