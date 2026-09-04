namespace WebcamStudio.Application.Interfaces.Services;

/// <summary>
/// Abstrae el envio de notificaciones por WhatsApp (hoy solo para avisos de prestamos).
/// La implementacion (ver WhatsAppCloudApiNotifier) nunca lanza excepcion: es una
/// notificacion secundaria, igual que IEmailSender -- si falla o las credenciales todavia
/// no estan configuradas, solo se registra en el log.
/// </summary>
public interface IWhatsAppNotifier
{
    Task SendAsync(string toPhoneNumber, string message, CancellationToken ct = default);
}
