using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Interfaces.Persistence;

public interface IStoreSaleRepository : IRepository<StoreSale>
{
    /// <summary>Ventas registradas, filtradas opcionalmente por modelo.</summary>
    Task<List<StoreSale>> SearchAsync(Guid? modelAccountId, CancellationToken ct = default);
}
