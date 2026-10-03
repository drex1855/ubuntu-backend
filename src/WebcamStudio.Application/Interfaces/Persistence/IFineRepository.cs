using WebcamStudio.Domain.Entities;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.Interfaces.Persistence;

public interface IFineRepository : IRepository<Fine>
{
    
    Task<List<Fine>> SearchAsync(Guid? modelAccountId, FineStatus? status, CancellationToken ct = default);
}
