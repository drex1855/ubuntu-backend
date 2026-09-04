using WebcamStudio.Application.Common;

namespace WebcamStudio.Application.Contacts;

public interface IContactService
{
    Task<Result<ContactDto>> SubmitPublicContactAsync(CreatePublicContactRequest request, CancellationToken ct = default);
    Task<Result<ContactDto>> CreateAsync(CreateContactRequest request, CancellationToken ct = default);
    Task<Result<ContactDto>> UpdateAsync(Guid id, UpdateContactRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<List<ContactDto>> SearchAsync(string? search, Guid? tagId, bool? hasEmail, CancellationToken ct = default);

    Task<List<TagDto>> GetTagsAsync(CancellationToken ct = default);
    Task<Result<TagDto>> CreateTagAsync(CreateTagRequest request, CancellationToken ct = default);
    Task<Result> DeleteTagAsync(Guid tagId, CancellationToken ct = default);
    Task<Result<ContactDto>> AssignTagAsync(Guid contactId, Guid tagId, CancellationToken ct = default);
    Task<Result<ContactDto>> RemoveTagAsync(Guid contactId, Guid tagId, CancellationToken ct = default);

    Task<Result<MassEmailResultDto>> SendMassEmailAsync(SendMassEmailRequest request, CancellationToken ct = default);
}
