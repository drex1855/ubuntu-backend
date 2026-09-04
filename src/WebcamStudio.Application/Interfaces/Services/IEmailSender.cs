namespace WebcamStudio.Application.Interfaces.Services;

/// <summary>
/// Abstrae el envio de correo (usado hoy solo por el modulo de Prestamos, para avisarle
/// al dueno del estudio de una solicitud nueva). La implementacion real
/// (Infrastructure/ExternalServices/SmtpEmailSender.cs) manda el correo por SMTP segun
/// la seccion "Email" de appsettings; si en el futuro se quiere cambiar de proveedor
/// (SendGrid, Mailgun, etc.) solo hay que crear otra implementacion de esta interfaz y
/// registrarla en Infrastructure/DependencyInjection.cs -- el resto del sistema no cambia.
/// </summary>
public interface IEmailSender
{
    Task SendAsync(string toEmail, string subject, string body, CancellationToken ct = default);
}
