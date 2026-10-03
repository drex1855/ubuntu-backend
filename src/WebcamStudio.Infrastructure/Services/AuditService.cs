using System.Text.Json;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public AuditService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task LogAsync(
        string module,
        string action,
        string? entityName = null,
        Guid? entityId = null,
        object? details = null,
        CancellationToken ct = default)
    {
        var log = new AuditLog
        {
            AccountId = _currentUserService.AccountId,
            Module = module,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            DetailsJson = details is null ? null : JsonSerializer.Serialize(details),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.AuditLogs.AddAsync(log, ct);
    }

    public async Task<List<AuditLog>> GetRecentAsync(string? module = null, int take = 100, CancellationToken ct = default)
    {
        var logs = await _unitOfWork.AuditLogs.GetAllAsync(ct);
        var query = logs.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(module))
            query = query.Where(l => l.Module == module);

        return query.OrderByDescending(l => l.CreatedAt).Take(take).ToList();
    }
}
