using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Interfaces.Persistence;

public interface IRoomRepository : IRepository<Room>
{
    Task<Room?> GetWithTemplateItemsAsync(Guid roomId, CancellationToken ct = default);
    Task<List<Room>> GetActiveAsync(CancellationToken ct = default);
}
