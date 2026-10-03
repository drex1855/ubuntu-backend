using WebcamStudio.Domain.Entities;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.Interfaces.Persistence;

public interface ILoanRequestRepository : IRepository<LoanRequest>
{
   
    Task<List<LoanRequest>> SearchAsync(LoanRequestStatus? status, CancellationToken ct = default);
}
