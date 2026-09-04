using Microsoft.EntityFrameworkCore;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Domain.Entities;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Infrastructure.Persistence.Repositories;

public class FineRepository : Repository<Fine>, IFineRepository
{
    public FineRepository(AppDbContext context) : base(context) { }

    public async Task<List<Fine>> SearchAsync(Guid? modelAccountId, FineStatus? status, CancellationToken ct = default)
    {
        var query = DbSet.AsNoTracking()
            .Include(f => f.ModelAccount)
            .AsQueryable();

        if (modelAccountId.HasValue)
            query = query.Where(f => f.ModelAccountId == modelAccountId.Value);

        if (status.HasValue)
            query = query.Where(f => f.Status == status.Value);

        return await query.OrderByDescending(f => f.IssuedAt).ToListAsync(ct);
    }
}
