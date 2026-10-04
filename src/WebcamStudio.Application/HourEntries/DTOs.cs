using System.ComponentModel.DataAnnotations;

namespace WebcamStudio.Application.HourEntries;

public record CreateHourEntryRequest(
    Guid ModelAccountId,
    [Range(-1440, 1440)] int Minutes,
    [MaxLength(500)] string? Note);

public record HourEntryDto(
    Guid Id,
    Guid ModelAccountId,
    string ModelFullName,
    int Minutes,
    string? Note,
    DateTime RegisteredAt);
