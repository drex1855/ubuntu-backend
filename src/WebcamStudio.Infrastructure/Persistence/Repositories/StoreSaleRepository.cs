using Microsoft.EntityFrameworkCore;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Repositories;

public class StoreSaleRepository : Repository<StoreSale>, IStoreSaleRepository
{
    public StoreSaleRepository(AppDbContext context) : base(context) { }

    public async Task<List<StoreSale>> SearchAsync(Guid? modelAccountId, CancellationToken ct = default)
    {
        var query = DbSet.AsNoTracking()
            .Include(s => s.Product)
            .Include(s => s.ModelAccount)
            .AsQueryable();

        if (modelAccountId.HasValue)
            query = query.Where(s => s.ModelAccountId == modelAccountId.Value);

        return await query.OrderByDescending(s => s.SoldAt).ToListAsync(ct);
    }
}
