namespace WebcamStudio.Infrastructure.ExternalServices;

/// <summary>
/// Se enlaza a la seccion "WhatsApp" de appsettings.json. Usa la API oficial de Meta
/// (WhatsApp Cloud API) -- ver WhatsAppCloudApiNotifier. PhoneNumberId/AccessToken salen
/// del panel de Meta for Developers al configurar un numero de WhatsApp Business; hasta
/// que se carguen valores reales, el envio simplemente se loguea sin fallar (mismo
/// criterio que EmailSettings).
/// </summary>
public class WhatsAppSettings
{
    public const string SectionName = "WhatsApp";

    public string PhoneNumberId { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string ApiVersion { get; set; } = "v21.0";
}
