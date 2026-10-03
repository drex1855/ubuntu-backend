using System.ComponentModel.DataAnnotations;

namespace WebcamStudio.Application.Sites;

public record SiteDto(Guid Id, string Name, string? Description, bool IsActive, decimal TokenValueUsd);

public record CreateSiteRequest(
    [Required, MaxLength(200)] string Name,
    [MaxLength(1000)] string? Description,
    [Range(0, 100_000)] decimal TokenValueUsd);

public record UpdateSiteRequest(
    [Required, MaxLength(200)] string Name,
    [MaxLength(1000)] string? Description,
    [Range(0, 100_000)] decimal TokenValueUsd);
