using Microsoft.EntityFrameworkCore;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public async Task<List<Product>> GetActiveAsync(CancellationToken ct = default) =>
        await DbSet.AsNoTracking().Where(p => p.IsActive).ToListAsync(ct);
}
