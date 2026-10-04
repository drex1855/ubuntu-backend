using System.ComponentModel.DataAnnotations;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.Fines;

public record FineDto(
    Guid Id,
    Guid ModelAccountId,
    string ModelFullName,
    decimal Amount,
    string Reason,
    FineStatus Status,
    DateTime IssuedAt,
    DateTime? ResolvedAt);

public record CreateFineRequest(
    Guid ModelAccountId,
    [Range(0.01, 1_000_000)] decimal Amount,
    [Required, MaxLength(500)] string Reason);

public record UpdateFineStatusRequest(FineStatus Status);
