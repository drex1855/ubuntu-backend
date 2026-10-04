namespace WebcamStudio.Application.Audit;

public record AuditLogDto(
    Guid Id,
    Guid? AccountId,
    string Module,
    string Action,
    string? EntityName,
    Guid? EntityId,
    string? DetailsJson,
    DateTime CreatedAt);
