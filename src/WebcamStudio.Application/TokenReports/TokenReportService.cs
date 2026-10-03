using WebcamStudio.Application.Common;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.TokenReports;


public class TokenReportService : ITokenReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public TokenReportService(IUnitOfWork unitOfWork, IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<Result<TokenReportDto>> CreateAsync(
        Guid registeredByAccountId, CreateTokenReportRequest request, CancellationToken ct = default)
    {
       
        var account = await _unitOfWork.ModelAccounts.GetByIdAsync(request.ModelAccountId, ct);
        if (account is null)
            return Result<TokenReportDto>.Failure("Modelo no encontrada.");

        
        var site = await _unitOfWork.Sites.GetByIdAsync(request.SiteId, ct);
        if (site is null)
            return Result<TokenReportDto>.Failure("Sitio no encontrado.");

        
        if (request.TokensAmount < 0)
            return Result<TokenReportDto>.Failure("La cantidad de tokens no puede ser negativa.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var normalizedPeriod = new DateOnly(request.Period.Year, request.Period.Month, 1);
        if (normalizedPeriod > new DateOnly(today.Year, today.Month, 1))
            return Result<TokenReportDto>.Failure("No se puede registrar un periodo futuro.");

        
        var monetaryValue = request.TokensAmount * site.TokenValueUsd;
        var report = new TokenReport
        {
            ModelAccountId = account.Id,
            SiteId = site.Id,
            Period = normalizedPeriod,
            TokensAmount = request.TokensAmount,
            MonetaryValue = monetaryValue,
            RegisteredByAccountId = registeredByAccountId,
            RegisteredAt = DateTime.UtcNow
        };

        await _unitOfWork.TokenReports.AddAsync(report, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        
        await _auditService.LogAsync("Tokens", "ReporteRegistrado", nameof(TokenReport), report.Id,
            new { request.TokensAmount, monetaryValue, Period = normalizedPeriod }, ct);

        return Result<TokenReportDto>.Success(new TokenReportDto(
            report.Id, account.Id, account.FullName, site.Id, site.Name,
            report.Period, report.TokensAmount, report.MonetaryValue, report.RegisteredAt));
    }

    public async Task<List<TokenReportDto>> SearchAsync(TokenReportSearchRequest request, CancellationToken ct = default)
    {
        var reports = await _unitOfWork.TokenReports.SearchAsync(
            request.ModelAccountId, request.SiteId, request.PeriodFrom, request.PeriodTo, ct);

        return reports.Select(r => new TokenReportDto(
            r.Id, r.ModelAccountId, r.ModelAccount.FullName, r.SiteId, r.Site.Name,
            r.Period, r.TokensAmount, r.MonetaryValue, r.RegisteredAt)).ToList();
    }

    public async Task<List<TokenSummaryDto>> GetSummaryAsync(TokenReportSearchRequest request, CancellationToken ct = default)
    {
        
        var reports = await _unitOfWork.TokenReports.SearchAsync(
            request.ModelAccountId, request.SiteId, request.PeriodFrom, request.PeriodTo, ct);

        return reports
            .GroupBy(r => (r.ModelAccountId, r.ModelAccount.FullName, r.SiteId, r.Site.Name))
            .Select(g => new TokenSummaryDto(
                g.Key.ModelAccountId, g.Key.FullName, g.Key.SiteId, g.Key.Name,
                g.Sum(r => r.TokensAmount), g.Sum(r => r.MonetaryValue), g.Count()))
            .ToList();
    }
}
