using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Interfaces.Persistence;

public interface ITokenReportRepository : IRepository<TokenReport>
{
    Task<List<TokenReport>> SearchAsync(
        Guid? modelAccountId,
        Guid? siteId,
        DateOnly? periodFrom,
        DateOnly? periodTo,
        CancellationToken ct = default);
}
