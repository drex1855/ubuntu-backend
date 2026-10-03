using Microsoft.EntityFrameworkCore;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Repositories;

public class ContactRepository : Repository<Contact>, IContactRepository
{
    public ContactRepository(AppDbContext context) : base(context) { }

    public async Task<List<Contact>> SearchAsync(string? search, Guid? tagId, bool? hasEmail, CancellationToken ct = default)
    {
        var query = DbSet.AsNoTracking().Include(c => c.Tags).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.FullName.Contains(search) || c.PhoneNumber.Contains(search));

        if (tagId.HasValue)
            query = query.Where(c => c.Tags.Any(t => t.Id == tagId.Value));

        if (hasEmail.HasValue)
            query = hasEmail.Value ? query.Where(c => c.Email != null) : query.Where(c => c.Email == null);

        return await query.OrderByDescending(c => c.CreatedAt).ToListAsync(ct);
    }

    public async Task<Contact?> GetByIdWithTagsAsync(Guid id, CancellationToken ct = default) =>
        await DbSet.Include(c => c.Tags).FirstOrDefaultAsync(c => c.Id == id, ct);
}
