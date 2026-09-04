using WebcamStudio.Application.Common;

namespace WebcamStudio.Application.Sites;

/// <summary>
/// CRUD de catalogo de sitios/plataformas (E2 del diagrama: "Seleccionar sitio o cuenta").
/// Se mantiene como modulo propio, chico y separado, porque es justo el tipo de pieza
/// que crece independiente: nuevos sitios se agregan por catalogo, no por codigo.
/// </summary>
public interface ISiteService
{
    Task<List<SiteDto>> GetAllAsync(CancellationToken ct = default);
    Task<Result<SiteDto>> CreateAsync(CreateSiteRequest request, CancellationToken ct = default);
    Task<Result<SiteDto>> UpdateAsync(Guid id, UpdateSiteRequest request, CancellationToken ct = default);
}
