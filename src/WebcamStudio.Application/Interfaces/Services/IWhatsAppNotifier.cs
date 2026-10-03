namespace WebcamStudio.Application.Interfaces.Services;


public interface IWhatsAppNotifier
{
    Task SendAsync(string toPhoneNumber, string message, CancellationToken ct = default);
}
