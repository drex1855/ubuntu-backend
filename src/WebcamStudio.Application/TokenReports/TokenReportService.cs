using WebcamStudio.Application.Common;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.TokenReports;

/// <summary>
/// Implementa el modulo "Reporte de tokens" (E) del diagrama:
/// E1 Seleccionar modelo -> E2 Seleccionar sitio o cuenta -> E3 Registrar tokens ->
/// E4 Validar periodo y monto -> E5 Guardar reporte -> E6 Generar resumen -> Z Auditoria.
/// </summary>
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
        // E1: Seleccionar modelo.
        var account = await _unitOfWork.ModelAccounts.GetByIdAsync(request.ModelAccountId, ct);
        if (account is null)
            return Result<TokenReportDto>.Failure("Modelo no encontrada.");

        // E2: Seleccionar sitio o cuenta.
        var site = await _unitOfWork.Sites.GetByIdAsync(request.SiteId, ct);
        if (site is null)
            return Result<TokenReportDto>.Failure("Sitio no encontrado.");

        // E4: Validar periodo y monto.
        if (request.TokensAmount < 0)
            return Result<TokenReportDto>.Failure("La cantidad de tokens no puede ser negativa.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var normalizedPeriod = new DateOnly(request.Period.Year, request.Period.Month, 1);
        if (normalizedPeriod > new DateOnly(today.Year, today.Month, 1))
            return Result<TokenReportDto>.Failure("No se puede registrar un periodo futuro.");

        // E3 + E5: Registrar tokens / Guardar reporte. El valor monetario se calcula, no se
        // ingresa a mano: cada sitio tiene su propia tasa (Site.TokenValueUsd).
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

        // Z: Registrar auditoria.
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
        // E6: Generar resumen por modelo y sitio. El agrupamiento se hace aqui, en memoria,
        // sobre los reportes ya filtrados -- si el volumen crece mucho, este es el punto
        // donde conviene mover el agrupamiento a una consulta agregada en la base de datos.
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
