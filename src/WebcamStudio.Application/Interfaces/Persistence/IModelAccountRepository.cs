using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Interfaces.Persistence;

public interface IModelAccountRepository : IRepository<ModelAccount>
{
    Task<ModelAccount?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<ModelAccount?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default);
}
