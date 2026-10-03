using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Interfaces.Persistence;

public interface IContactRepository : IRepository<Contact>
{
  
    Task<List<Contact>> SearchAsync(string? search, Guid? tagId, bool? hasEmail, CancellationToken ct = default);

   
    Task<Contact?> GetByIdWithTagsAsync(Guid id, CancellationToken ct = default);
}
