using WebcamStudio.Application.Common;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.Fines;

public interface IFineService
{
    Task<Result<FineDto>> CreateAsync(CreateFineRequest request, CancellationToken ct = default);
    Task<List<FineDto>> SearchAsync(Guid? modelAccountId, FineStatus? status, CancellationToken ct = default);
    Task<Result<FineDto>> SetStatusAsync(Guid id, UpdateFineStatusRequest request, CancellationToken ct = default);

    Task<List<FineDto>> GetMyFinesAsync(Guid modelAccountId, CancellationToken ct = default);
}
