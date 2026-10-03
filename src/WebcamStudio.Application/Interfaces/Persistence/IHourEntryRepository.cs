using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Interfaces.Persistence;

public interface IHourEntryRepository : IRepository<HourEntry>
{
    
    Task<List<HourEntry>> SearchAsync(Guid? modelAccountId, CancellationToken ct = default);
}
