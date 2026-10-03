using Microsoft.EntityFrameworkCore;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Repositories;

public class TokenReportRepository : Repository<TokenReport>, ITokenReportRepository
{
    public TokenReportRepository(AppDbContext context) : base(context) { }

    public async Task<List<TokenReport>> SearchAsync(
        Guid? modelAccountId, Guid? siteId, DateOnly? periodFrom, DateOnly? periodTo, CancellationToken ct = default)
    {
        var query = DbSet.AsNoTracking()
            .Include(r => r.ModelAccount)
            .Include(r => r.Site)
            .AsQueryable();

        if (modelAccountId.HasValue)
            query = query.Where(r => r.ModelAccountId == modelAccountId.Value);

        if (siteId.HasValue)
            query = query.Where(r => r.SiteId == siteId.Value);

        if (periodFrom.HasValue)
            query = query.Where(r => r.Period >= periodFrom.Value);

        if (periodTo.HasValue)
            query = query.Where(r => r.Period <= periodTo.Value);

        return await query.OrderByDescending(r => r.Period).ToListAsync(ct);
    }
}
