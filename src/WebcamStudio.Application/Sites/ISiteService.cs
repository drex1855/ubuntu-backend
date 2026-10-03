using WebcamStudio.Application.Common;

namespace WebcamStudio.Application.Sites;


public interface ISiteService
{
    Task<List<SiteDto>> GetAllAsync(CancellationToken ct = default);
    Task<Result<SiteDto>> CreateAsync(CreateSiteRequest request, CancellationToken ct = default);
    Task<Result<SiteDto>> UpdateAsync(Guid id, UpdateSiteRequest request, CancellationToken ct = default);
}
