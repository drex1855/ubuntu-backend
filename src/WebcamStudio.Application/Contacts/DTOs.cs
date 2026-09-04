using System.ComponentModel.DataAnnotations;

namespace WebcamStudio.Application.Contacts;

/// <summary>Payload del formulario publico de contacto en el sitio (sin autenticacion).</summary>
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

/// <summary>
/// Resultado de un envio masivo. "Sent" refleja cuantos correos se INTENTARON enviar,
/// no cuantos llegaron de verdad: IEmailSender.SendAsync (ver SmtpEmailSender) nunca
/// lanza excepcion ni reporta exito/fallo al llamador, asi que no hay forma de saber
/// desde aqui si un envio puntual fallo. No mostrar esto en la UI como "N enviados
/// exitosamente" -- ver comentario en ContactService.SendMassEmailAsync.
/// </summary>
public record MassEmailResultDto(int Recipients, int Sent, int SkippedNoEmail);
