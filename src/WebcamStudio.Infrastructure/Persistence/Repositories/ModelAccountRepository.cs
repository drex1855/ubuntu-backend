using Microsoft.EntityFrameworkCore;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Repositories;

public class ModelAccountRepository : Repository<ModelAccount>, IModelAccountRepository
{
    public ModelAccountRepository(AppDbContext context) : base(context) { }

    public async Task<ModelAccount?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        await DbSet.FirstOrDefaultAsync(a => a.Email == email, ct);

    public async Task<ModelAccount?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default) =>
        await DbSet.FirstOrDefaultAsync(a => a.PhoneNumber == phoneNumber, ct);
}
