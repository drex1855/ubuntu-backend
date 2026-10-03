using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebcamStudio.Application.Interfaces.Services;

namespace WebcamStudio.Infrastructure.ExternalServices;

public class WhatsAppCloudApiNotifier : IWhatsAppNotifier
{
    private readonly HttpClient _httpClient;
    private readonly WhatsAppSettings _settings;
    private readonly ILogger<WhatsAppCloudApiNotifier> _logger;

    public WhatsAppCloudApiNotifier(HttpClient httpClient, IOptions<WhatsAppSettings> settings, ILogger<WhatsAppCloudApiNotifier> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendAsync(string toPhoneNumber, string message, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(toPhoneNumber))
            return;

        if (string.IsNullOrWhiteSpace(_settings.PhoneNumberId) || string.IsNullOrWhiteSpace(_settings.AccessToken))
        {
            _logger.LogWarning(
                "No se envio el WhatsApp a {ToPhoneNumber}: falta configurar la seccion 'WhatsApp' en appsettings.",
                toPhoneNumber);
            return;
        }

        try
        {
            var url = $"https://graph.facebook.com/{_settings.ApiVersion}/{_settings.PhoneNumberId}/messages";
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.AccessToken);
            request.Content = JsonContent.Create(new
            {
                messaging_product = "whatsapp",
                to = toPhoneNumber,
                type = "text",
                text = new { body = message }
            });

            var response = await _httpClient.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError(
                    "No se pudo enviar el WhatsApp a {ToPhoneNumber}. Status {Status}: {Body}",
                    toPhoneNumber, response.StatusCode, body);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudo enviar el WhatsApp a {ToPhoneNumber}.", toPhoneNumber);
        }
    }
}
