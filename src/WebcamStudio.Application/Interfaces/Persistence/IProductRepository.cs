using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Interfaces.Persistence;

public interface IProductRepository : IRepository<Product>
{
    Task<List<Product>> GetActiveAsync(CancellationToken ct = default);
}
