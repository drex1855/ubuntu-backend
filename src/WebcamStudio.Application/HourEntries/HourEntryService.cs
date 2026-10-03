using WebcamStudio.Application.Common;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.HourEntries;


public class HourEntryService : IHourEntryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public HourEntryService(IUnitOfWork unitOfWork, IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<Result<HourEntryDto>> CreateAsync(
        Guid registeredByAccountId, CreateHourEntryRequest request, CancellationToken ct = default)
    {
        if (request.Minutes == 0)
            return Result<HourEntryDto>.Failure("La cantidad de horas no puede ser cero.");

        var account = await _unitOfWork.ModelAccounts.GetByIdAsync(request.ModelAccountId, ct);
        if (account is null)
            return Result<HourEntryDto>.Failure("Cuenta no encontrada.");

        var entry = new HourEntry
        {
            ModelAccountId = account.Id,
            Minutes = request.Minutes,
            Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim(),
            RegisteredByAccountId = registeredByAccountId,
            RegisteredAt = DateTime.UtcNow
        };

        await _unitOfWork.HourEntries.AddAsync(entry, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Horas", request.Minutes > 0 ? "HorasSumadas" : "HorasRestadas",
            nameof(HourEntry), entry.Id, new { request.ModelAccountId, request.Minutes }, ct);

        return Result<HourEntryDto>.Success(ToDto(entry, account.FullName));
    }

    public async Task<List<HourEntryDto>> SearchAsync(Guid? modelAccountId, CancellationToken ct = default)
    {
        var entries = await _unitOfWork.HourEntries.SearchAsync(modelAccountId, ct);
        return entries.Select(e => ToDto(e, e.ModelAccount.FullName)).ToList();
    }

    public async Task<List<HourEntryDto>> GetMineAsync(Guid modelAccountId, CancellationToken ct = default)
    {
        var entries = await _unitOfWork.HourEntries.SearchAsync(modelAccountId, ct);
        return entries.Select(e => ToDto(e, e.ModelAccount.FullName)).ToList();
    }

    private static HourEntryDto ToDto(HourEntry e, string modelFullName) =>
        new(e.Id, e.ModelAccountId, modelFullName, e.Minutes, e.Note, e.RegisteredAt);
}
