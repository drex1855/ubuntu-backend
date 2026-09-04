using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Interfaces.Persistence;

public interface IHourEntryRepository : IRepository<HourEntry>
{
    /// <summary>Movimientos filtrados opcionalmente por modelo, mas recientes primero.</summary>
    Task<List<HourEntry>> SearchAsync(Guid? modelAccountId, CancellationToken ct = default);
}
