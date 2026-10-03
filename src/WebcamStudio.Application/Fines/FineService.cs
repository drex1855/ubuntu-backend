using WebcamStudio.Application.Common;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Entities;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.Fines;


public class FineService : IFineService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public FineService(IUnitOfWork unitOfWork, IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<Result<FineDto>> CreateAsync(CreateFineRequest request, CancellationToken ct = default)
    {
        if (request.Amount <= 0)
            return Result<FineDto>.Failure("El monto debe ser mayor a cero.");
        if (string.IsNullOrWhiteSpace(request.Reason))
            return Result<FineDto>.Failure("El motivo de la multa es obligatorio.");

        var account = await _unitOfWork.ModelAccounts.GetByIdAsync(request.ModelAccountId, ct);
        if (account is null)
            return Result<FineDto>.Failure("Cuenta no encontrada.");

        var fine = new Fine
        {
            ModelAccountId = account.Id,
            Amount = request.Amount,
            Reason = request.Reason.Trim(),
            Status = FineStatus.PendientePorCobrar,
            IssuedAt = DateTime.UtcNow
        };

        await _unitOfWork.Fines.AddAsync(fine, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Multas", "MultaCreada", nameof(Fine), fine.Id,
            new { request.ModelAccountId, request.Amount }, ct);

        return Result<FineDto>.Success(ToDto(fine, account.FullName));
    }

    public async Task<List<FineDto>> SearchAsync(Guid? modelAccountId, FineStatus? status, CancellationToken ct = default)
    {
        var fines = await _unitOfWork.Fines.SearchAsync(modelAccountId, status, ct);
        return fines.Select(f => ToDto(f, f.ModelAccount.FullName)).ToList();
    }

    public async Task<Result<FineDto>> SetStatusAsync(Guid id, UpdateFineStatusRequest request, CancellationToken ct = default)
    {
        var fine = await _unitOfWork.Fines.GetByIdAsync(id, ct);
        if (fine is null)
            return Result<FineDto>.Failure("Multa no encontrada.");

        var account = await _unitOfWork.ModelAccounts.GetByIdAsync(fine.ModelAccountId, ct);

        fine.Status = request.Status;
        fine.ResolvedAt = request.Status == FineStatus.PendientePorCobrar ? null : DateTime.UtcNow;

        _unitOfWork.Fines.Update(fine);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Multas", $"EstadoCambiadoA{request.Status}", nameof(Fine), fine.Id, null, ct);

        return Result<FineDto>.Success(ToDto(fine, account?.FullName ?? string.Empty));
    }

    public async Task<List<FineDto>> GetMyFinesAsync(Guid modelAccountId, CancellationToken ct = default)
    {
        var fines = await _unitOfWork.Fines.SearchAsync(modelAccountId, null, ct);
        return fines.Select(f => ToDto(f, f.ModelAccount.FullName)).ToList();
    }

    private static FineDto ToDto(Fine f, string modelFullName) => new(
        f.Id, f.ModelAccountId, modelFullName, f.Amount, f.Reason, f.Status, f.IssuedAt, f.ResolvedAt);
}
