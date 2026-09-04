using Microsoft.EntityFrameworkCore;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Repositories;

public class HourEntryRepository : Repository<HourEntry>, IHourEntryRepository
{
    public HourEntryRepository(AppDbContext context) : base(context) { }

    public async Task<List<HourEntry>> SearchAsync(Guid? modelAccountId, CancellationToken ct = default)
    {
        var query = DbSet.AsNoTracking()
            .Include(h => h.ModelAccount)
            .AsQueryable();

        if (modelAccountId.HasValue)
            query = query.Where(h => h.ModelAccountId == modelAccountId.Value);

        return await query.OrderByDescending(h => h.RegisteredAt).ToListAsync(ct);
    }
}
