using WebcamStudio.Application.Common;

namespace WebcamStudio.Application.HourEntries;

public interface IHourEntryService
{
    Task<Result<HourEntryDto>> CreateAsync(Guid registeredByAccountId, CreateHourEntryRequest request, CancellationToken ct = default);

    Task<List<HourEntryDto>> SearchAsync(Guid? modelAccountId, CancellationToken ct = default);

    Task<List<HourEntryDto>> GetMineAsync(Guid modelAccountId, CancellationToken ct = default);
}
