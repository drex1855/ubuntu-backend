using Microsoft.EntityFrameworkCore;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Repositories;

public class ChecklistRunRepository : Repository<ChecklistRun>, IChecklistRunRepository
{
    public ChecklistRunRepository(AppDbContext context) : base(context) { }

    public async Task<ChecklistRun?> GetWithDetailsAsync(Guid id, CancellationToken ct = default) =>
        await DbSet
            .Include(r => r.Room)
            .Include(r => r.ItemResults).ThenInclude(i => i.TemplateItem)
            .Include(r => r.MaintenanceRequests)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<List<ChecklistRun>> GetByRoomAsync(Guid roomId, CancellationToken ct = default) =>
        await DbSet.AsNoTracking()
            .Where(r => r.RoomId == roomId)
            .OrderByDescending(r => r.PerformedAt)
            .ToListAsync(ct);
}
