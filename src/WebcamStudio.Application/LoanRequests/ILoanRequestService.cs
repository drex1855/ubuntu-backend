using WebcamStudio.Application.Common;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.LoanRequests;

public interface ILoanRequestService
{
    Task<Result<LoanRequestDto>> CreateAsync(
        Guid requestedByAccountId, CreateLoanRequestRequest request, CancellationToken ct = default);

    Task<List<LoanRequestDto>> SearchAsync(LoanRequestStatus? status, CancellationToken ct = default);

    Task<Result<LoanRequestDto>> SetStatusAsync(
        Guid id, UpdateLoanRequestStatusRequest request, CancellationToken ct = default);
}
