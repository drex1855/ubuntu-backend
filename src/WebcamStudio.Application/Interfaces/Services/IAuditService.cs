using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Interfaces.Services;

/// <summary>
/// Servicio transversal de auditoria. Corresponde al nodo "Registrar auditoria" (Z)
/// del diagrama, al que convergen los 5 modulos antes de responder al frontend.
/// Cualquier servicio de cualquier modulo (existente o futuro) llama a LogAsync
/// en vez de escribir directo a la tabla AuditLog, para mantener un solo formato
/// de auditoria en todo el sistema.
/// </summary>
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
