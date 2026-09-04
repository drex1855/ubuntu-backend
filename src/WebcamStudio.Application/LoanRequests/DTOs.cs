using System.ComponentModel.DataAnnotations;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.LoanRequests;

public record LoanRequestDto(
    Guid Id,
    Guid RequestedByAccountId,
    string RequestedByFullName,
    decimal Amount,
    string Reason,
    LoanRequestStatus Status,
    DateTime RequestedAt,
    DateTime? ResolvedAt);

public record CreateLoanRequestRequest([Range(0.01, 1_000_000)] decimal Amount, [Required, MaxLength(500)] string Reason);

public record UpdateLoanRequestStatusRequest(LoanRequestStatus Status);
