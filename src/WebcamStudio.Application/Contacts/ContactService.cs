using System.Net.Mail;
using WebcamStudio.Application.Common;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Contacts;


public class ContactService : IContactService
{
    private const int FullNameMaxLength = 150;
    private const int PhoneNumberMaxLength = 30;
    private const int EmailMaxLength = 200;
    private const int NotesMaxLength = 2000;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;
    private readonly IEmailSender _emailSender;

    public ContactService(IUnitOfWork unitOfWork, IAuditService auditService, IEmailSender emailSender)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
        _emailSender = emailSender;
    }

    public async Task<Result<ContactDto>> SubmitPublicContactAsync(CreatePublicContactRequest request, CancellationToken ct = default)
    {
        var validation = ValidateFields(request.FullName, request.PhoneNumber, request.Email);
        if (validation is not null)
            return Result<ContactDto>.Failure(validation);

        if (!request.Consent)
            return Result<ContactDto>.Failure("Debes aceptar el uso de tus datos para continuar.");

        var now = DateTime.UtcNow;
        var contact = new Contact
        {
            FullName = request.FullName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            Source = "SitioWeb",
            ConsentGiven = true,
            ConsentGivenAt = now
        };

        await _unitOfWork.Contacts.AddAsync(contact, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Contactos", "ContactoCreado", nameof(Contact), contact.Id,
            new { contact.PhoneNumber }, ct);

        return Result<ContactDto>.Success(ToDto(contact));
    }

    public async Task<Result<ContactDto>> CreateAsync(CreateContactRequest request, CancellationToken ct = default)
    {
        var validation = ValidateFields(request.FullName, request.PhoneNumber, request.Email)
            ?? ValidateNotes(request.Notes);
        if (validation is not null)
            return Result<ContactDto>.Failure(validation);

        var contact = new Contact
        {
            FullName = request.FullName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            Source = "Manual"
        };

        await _unitOfWork.Contacts.AddAsync(contact, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Contactos", "ContactoCreado", nameof(Contact), contact.Id,
            new { contact.PhoneNumber }, ct);

        return Result<ContactDto>.Success(ToDto(contact));
    }

    public async Task<Result<ContactDto>> UpdateAsync(Guid id, UpdateContactRequest request, CancellationToken ct = default)
    {
        var validation = ValidateFields(request.FullName, request.PhoneNumber, request.Email)
            ?? ValidateNotes(request.Notes);
        if (validation is not null)
            return Result<ContactDto>.Failure(validation);

        var contact = await _unitOfWork.Contacts.GetByIdWithTagsAsync(id, ct);
        if (contact is null)
            return Result<ContactDto>.Failure("Contacto no encontrado.");

        contact.FullName = request.FullName.Trim();
        contact.PhoneNumber = request.PhoneNumber.Trim();
        contact.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        contact.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();

        _unitOfWork.Contacts.Update(contact);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Contactos", "ContactoActualizado", nameof(Contact), contact.Id, null, ct);

        return Result<ContactDto>.Success(ToDto(contact));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var contact = await _unitOfWork.Contacts.GetByIdAsync(id, ct);
        if (contact is null)
            return Result.Failure("Contacto no encontrado.");

        await _auditService.LogAsync("Contactos", "ContactoEliminado", nameof(Contact), contact.Id,
            new { contact.FullName, contact.PhoneNumber }, ct);

        _unitOfWork.Contacts.Remove(contact);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<List<ContactDto>> SearchAsync(string? search, Guid? tagId, bool? hasEmail, CancellationToken ct = default)
    {
        var contacts = await _unitOfWork.Contacts.SearchAsync(search, tagId, hasEmail, ct);
        return contacts.Select(ToDto).ToList();
    }

    public async Task<List<TagDto>> GetTagsAsync(CancellationToken ct = default)
    {
        var tags = await _unitOfWork.Tags.GetAllAsync(ct);
        return tags.OrderBy(t => t.Name).Select(t => new TagDto(t.Id, t.Name)).ToList();
    }

    public async Task<Result<TagDto>> CreateTagAsync(CreateTagRequest request, CancellationToken ct = default)
    {
        var name = request.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
            return Result<TagDto>.Failure("El nombre de la etiqueta es obligatorio.");

        var existing = await _unitOfWork.Tags.GetAllAsync(ct);
        if (existing.Any(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase)))
            return Result<TagDto>.Failure("Ya existe una etiqueta con ese nombre.");

        var tag = new Tag { Name = name };
        await _unitOfWork.Tags.AddAsync(tag, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Contactos", "EtiquetaCreada", nameof(Tag), tag.Id, new { tag.Name }, ct);

        return Result<TagDto>.Success(new TagDto(tag.Id, tag.Name));
    }

    public async Task<Result> DeleteTagAsync(Guid tagId, CancellationToken ct = default)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(tagId, ct);
        if (tag is null)
            return Result.Failure("Etiqueta no encontrada.");

        await _auditService.LogAsync("Contactos", "EtiquetaEliminada", nameof(Tag), tag.Id, new { tag.Name }, ct);

        _unitOfWork.Tags.Remove(tag);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result<ContactDto>> AssignTagAsync(Guid contactId, Guid tagId, CancellationToken ct = default)
    {
        var contact = await _unitOfWork.Contacts.GetByIdWithTagsAsync(contactId, ct);
        if (contact is null)
            return Result<ContactDto>.Failure("Contacto no encontrado.");

        var tag = await _unitOfWork.Tags.GetByIdAsync(tagId, ct);
        if (tag is null)
            return Result<ContactDto>.Failure("Etiqueta no encontrada.");

        if (contact.Tags.All(t => t.Id != tagId))
            contact.Tags.Add(tag);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<ContactDto>.Success(ToDto(contact));
    }

    public async Task<Result<ContactDto>> RemoveTagAsync(Guid contactId, Guid tagId, CancellationToken ct = default)
    {
        var contact = await _unitOfWork.Contacts.GetByIdWithTagsAsync(contactId, ct);
        if (contact is null)
            return Result<ContactDto>.Failure("Contacto no encontrado.");

        var tag = contact.Tags.FirstOrDefault(t => t.Id == tagId);
        if (tag is not null)
            contact.Tags.Remove(tag);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<ContactDto>.Success(ToDto(contact));
    }

    
    public async Task<Result<MassEmailResultDto>> SendMassEmailAsync(SendMassEmailRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Subject) || string.IsNullOrWhiteSpace(request.Body))
            return Result<MassEmailResultDto>.Failure("El asunto y el cuerpo del correo son obligatorios.");

        var recipients = new Dictionary<Guid, Contact>();

        if (request.ContactIds is { Count: > 0 })
        {
            foreach (var id in request.ContactIds)
            {
                var contact = await _unitOfWork.Contacts.GetByIdAsync(id, ct);
                if (contact is not null)
                    recipients[contact.Id] = contact;
            }
        }

        if (request.TagId.HasValue)
        {
            var tagged = await _unitOfWork.Contacts.SearchAsync(null, request.TagId, null, ct);
            foreach (var contact in tagged)
                recipients[contact.Id] = contact;
        }

        if (recipients.Count == 0)
            return Result<MassEmailResultDto>.Failure("No hay destinatarios seleccionados.");

        var withEmail = recipients.Values.Where(c => !string.IsNullOrWhiteSpace(c.Email)).ToList();
        var skippedNoEmail = recipients.Count - withEmail.Count;

        foreach (var contact in withEmail)
            await _emailSender.SendAsync(contact.Email!, request.Subject, request.Body, ct);

        await _auditService.LogAsync("Contactos", "CorreoMasivoEnviado", null, null,
            new { request.Subject, Recipients = withEmail.Count }, ct);

        return Result<MassEmailResultDto>.Success(new MassEmailResultDto(recipients.Count, withEmail.Count, skippedNoEmail));
    }

    private static string? ValidateFields(string fullName, string phoneNumber, string? email)
    {
        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phoneNumber))
            return "El nombre y el telefono son obligatorios.";

        if (fullName.Trim().Length > FullNameMaxLength)
            return $"El nombre no puede tener mas de {FullNameMaxLength} caracteres.";

        if (phoneNumber.Trim().Length > PhoneNumberMaxLength)
            return $"El telefono no puede tener mas de {PhoneNumberMaxLength} caracteres.";

        if (!string.IsNullOrWhiteSpace(email))
        {
            var trimmedEmail = email.Trim();
            if (trimmedEmail.Length > EmailMaxLength)
                return $"El correo no puede tener mas de {EmailMaxLength} caracteres.";
            if (!MailAddress.TryCreate(trimmedEmail, out _))
                return "El correo no tiene un formato valido.";
        }

        return null;
    }

    private static string? ValidateNotes(string? notes) =>
        !string.IsNullOrWhiteSpace(notes) && notes.Trim().Length > NotesMaxLength
            ? $"Las notas no pueden tener mas de {NotesMaxLength} caracteres."
            : null;

    private static ContactDto ToDto(Contact c) => new(
        c.Id, c.FullName, c.PhoneNumber, c.Email, c.Notes, c.Source,
        c.ConsentGiven, c.ConsentGivenAt, c.CreatedAt,
        c.Tags.Select(t => new TagDto(t.Id, t.Name)).OrderBy(t => t.Name).ToList());
}
