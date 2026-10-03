using Microsoft.EntityFrameworkCore;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Repositories;

public class RoomRepository : Repository<Room>, IRoomRepository
{
    public RoomRepository(AppDbContext context) : base(context) { }

    public async Task<Room?> GetWithTemplateItemsAsync(Guid roomId, CancellationToken ct = default) =>
        await DbSet.Include(r => r.TemplateItems).FirstOrDefaultAsync(r => r.Id == roomId, ct);

    public async Task<List<Room>> GetActiveAsync(CancellationToken ct = default) =>
        await DbSet.AsNoTracking().Where(r => r.IsActive).ToListAsync(ct);
}
