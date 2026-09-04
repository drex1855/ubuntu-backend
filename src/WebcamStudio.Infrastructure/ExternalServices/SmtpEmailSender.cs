using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebcamStudio.Application.Interfaces.Services;

namespace WebcamStudio.Infrastructure.ExternalServices;

/// <summary>
/// Envio real de correo por SMTP usando System.Net.Mail (parte del framework de .NET,
/// sin paquete NuGet de terceros -- mismo criterio que PasswordHasher). El cuerpo se
/// manda siempre en texto plano (IsBodyHtml = false) para no abrir superficie de
/// inyeccion HTML con contenido escrito por el usuario (ej. el motivo del prestamo).
///
/// Nunca lanza excepcion hacia el llamador: un correo es una notificacion secundaria,
/// si falla (SMTP mal configurado, timeout, etc.) solo se registra en el log y listo --
/// la capa Application (que no conoce logging/SMTP a proposito) no tiene que lidiar
/// con ese caso.
/// </summary>
public class SmtpEmailSender : IEmailSender
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<EmailSettings> settings, ILogger<SmtpEmailSender> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string subject, string body, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(_settings.SmtpHost))
        {
            _logger.LogWarning(
                "No se envio el correo '{Subject}': falta configurar Email/Owner en appsettings.", subject);
            return;
        }

        try
        {
            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.SmtpUsername, _settings.SmtpPassword)
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromAddress, _settings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            message.To.Add(toEmail);

            await client.SendMailAsync(message, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudo enviar el correo '{Subject}' a {ToEmail}.", subject, toEmail);
        }
    }
}
