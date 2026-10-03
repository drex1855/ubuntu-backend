using System.ComponentModel.DataAnnotations;

namespace WebcamStudio.Application.Contacts;
public record CreatePublicContactRequest(
    [Required, MaxLength(200)] string FullName,
    [Required, Phone, MaxLength(30)] string PhoneNumber,
    [EmailAddress, MaxLength(256)] string? Email,
    bool Consent);

public record CreateContactRequest(
    [Required, MaxLength(200)] string FullName,
    [Required, Phone, MaxLength(30)] string PhoneNumber,
    [EmailAddress, MaxLength(256)] string? Email,
    [MaxLength(2000)] string? Notes);

public record UpdateContactRequest(
    [Required, MaxLength(200)] string FullName,
    [Required, Phone, MaxLength(30)] string PhoneNumber,
    [EmailAddress, MaxLength(256)] string? Email,
    [MaxLength(2000)] string? Notes);

public record TagDto(Guid Id, string Name);

public record CreateTagRequest([Required, MaxLength(100)] string Name);

public record ContactDto(
    Guid Id,
    string FullName,
    string PhoneNumber,
    string? Email,
    string? Notes,
    string Source,
    bool ConsentGiven,
    DateTime ConsentGivenAt,
    DateTime CreatedAt,
    List<TagDto> Tags);

public record SendMassEmailRequest(
    [Required, MaxLength(300)] string Subject,
    [Required, MaxLength(20000)] string Body,
    List<Guid>? ContactIds,
    Guid? TagId);

public record MassEmailResultDto(int Recipients, int Sent, int SkippedNoEmail);
