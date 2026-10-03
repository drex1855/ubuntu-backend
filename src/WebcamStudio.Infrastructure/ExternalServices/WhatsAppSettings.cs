namespace WebcamStudio.Infrastructure.ExternalServices;

public class WhatsAppSettings
{
    public const string SectionName = "WhatsApp";

    public string PhoneNumberId { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string ApiVersion { get; set; } = "v21.0";
}
