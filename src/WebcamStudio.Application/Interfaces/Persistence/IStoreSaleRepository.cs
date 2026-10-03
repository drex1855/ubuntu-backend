using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Interfaces.Persistence;

public interface IStoreSaleRepository : IRepository<StoreSale>
{

    Task<List<StoreSale>> SearchAsync(Guid? modelAccountId, CancellationToken ct = default);
}
