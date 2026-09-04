using WebcamStudio.Application.Common;

namespace WebcamStudio.Application.TokenReports;

public interface ITokenReportService
{
    Task<Result<TokenReportDto>> CreateAsync(Guid registeredByAccountId, CreateTokenReportRequest request, CancellationToken ct = default);
    Task<List<TokenReportDto>> SearchAsync(TokenReportSearchRequest request, CancellationToken ct = default);
    Task<List<TokenSummaryDto>> GetSummaryAsync(TokenReportSearchRequest request, CancellationToken ct = default);
}
