using WebcamStudio.Application.Common;

namespace WebcamStudio.Application.HourEntries;

public interface IHourEntryService
{
    Task<Result<HourEntryDto>> CreateAsync(Guid registeredByAccountId, CreateHourEntryRequest request, CancellationToken ct = default);

    /// <summary>Historial de cualquier modelo (o de todas si no se filtra) -- solo staff.</summary>
    Task<List<HourEntryDto>> SearchAsync(Guid? modelAccountId, CancellationToken ct = default);

    /// <summary>Horas de la propia cuenta, solo lectura (ver ProfilePage).</summary>
    Task<List<HourEntryDto>> GetMineAsync(Guid modelAccountId, CancellationToken ct = default);
}
