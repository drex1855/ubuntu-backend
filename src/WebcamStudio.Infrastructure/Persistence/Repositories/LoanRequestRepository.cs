using Microsoft.EntityFrameworkCore;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Domain.Entities;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Infrastructure.Persistence.Repositories;

public class LoanRequestRepository : Repository<LoanRequest>, ILoanRequestRepository
{
    public LoanRequestRepository(AppDbContext context) : base(context) { }

    public async Task<List<LoanRequest>> SearchAsync(LoanRequestStatus? status, CancellationToken ct = default)
    {
        var query = DbSet.AsNoTracking()
            .Include(r => r.RequestedByAccount)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        return await query.OrderByDescending(r => r.RequestedAt).ToListAsync(ct);
    }
}
