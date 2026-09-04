using WebcamStudio.Domain.Entities;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.Interfaces.Persistence;

public interface ILoanRequestRepository : IRepository<LoanRequest>
{
    /// <summary>Trae solicitudes filtradas opcionalmente por estado, para el historial del Admin.</summary>
    Task<List<LoanRequest>> SearchAsync(LoanRequestStatus? status, CancellationToken ct = default);
}
