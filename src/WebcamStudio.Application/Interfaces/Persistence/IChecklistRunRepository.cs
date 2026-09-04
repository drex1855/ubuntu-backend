using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Interfaces.Persistence;

public interface IChecklistRunRepository : IRepository<ChecklistRun>
{
    Task<ChecklistRun?> GetWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<List<ChecklistRun>> GetByRoomAsync(Guid roomId, CancellationToken ct = default);
}
