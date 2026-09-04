using WebcamStudio.Application.Common;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Sites;

public class SiteService : ISiteService
{
    private readonly IUnitOfWork _unitOfWork;

    public SiteService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<SiteDto>> GetAllAsync(CancellationToken ct = default)
    {
        var sites = await _unitOfWork.Sites.GetAllAsync(ct);
        return sites.Select(ToDto).ToList();
    }

    public async Task<Result<SiteDto>> CreateAsync(CreateSiteRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<SiteDto>.Failure("El nombre del sitio es obligatorio.");
        if (request.TokenValueUsd < 0)
            return Result<SiteDto>.Failure("El valor por token no puede ser negativo.");

        var site = new Site
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = true,
            TokenValueUsd = request.TokenValueUsd
        };
        await _unitOfWork.Sites.AddAsync(site, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<SiteDto>.Success(ToDto(site));
    }

    public async Task<Result<SiteDto>> UpdateAsync(Guid id, UpdateSiteRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<SiteDto>.Failure("El nombre del sitio es obligatorio.");
        if (request.TokenValueUsd < 0)
            return Result<SiteDto>.Failure("El valor por token no puede ser negativo.");

        var site = await _unitOfWork.Sites.GetByIdAsync(id, ct);
        if (site is null)
            return Result<SiteDto>.Failure("Sitio no encontrado.");

        site.Name = request.Name;
        site.Description = request.Description;
        site.TokenValueUsd = request.TokenValueUsd;
        _unitOfWork.Sites.Update(site);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<SiteDto>.Success(ToDto(site));
    }

    private static SiteDto ToDto(Site s) => new(s.Id, s.Name, s.Description, s.IsActive, s.TokenValueUsd);
}
