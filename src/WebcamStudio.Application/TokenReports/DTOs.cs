using System.ComponentModel.DataAnnotations;

namespace WebcamStudio.Application.TokenReports;

public record CreateTokenReportRequest(
    Guid ModelAccountId,
    Guid SiteId,
    DateOnly Period,
    [Range(0, 1_000_000)] decimal TokensAmount);

public record TokenReportDto(
    Guid Id,
    Guid ModelAccountId,
    string ModelFullName,
    Guid SiteId,
    string SiteName,
    DateOnly Period,
    decimal TokensAmount,
    decimal MonetaryValue,
    DateTime RegisteredAt);


public record TokenSummaryDto(
    Guid ModelAccountId,
    string ModelFullName,
    Guid SiteId,
    string SiteName,
    decimal TotalTokens,
    decimal TotalMonetaryValue,
    int ReportsCount);

public record TokenReportSearchRequest(Guid? ModelAccountId, Guid? SiteId, DateOnly? PeriodFrom, DateOnly? PeriodTo);
