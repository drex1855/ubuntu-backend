using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Interfaces.Services;


public interface IAuditService
{
    Task LogAsync(
        string module,
        string action,
        string? entityName = null,
        Guid? entityId = null,
        object? details = null,
        CancellationToken ct = default);

    Task<List<AuditLog>> GetRecentAsync(string? module = null, int take = 100, CancellationToken ct = default);
}
