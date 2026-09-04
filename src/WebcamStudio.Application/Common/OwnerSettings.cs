namespace WebcamStudio.Application.Common;

/// <summary>
/// Se enlaza a la seccion "Owner" de appsettings.json. Se usa para saber a que correo y
/// numero de WhatsApp avisarle al dueno del estudio cuando llega una solicitud de
/// prestamo nueva (ver LoanRequestService).
/// </summary>
public class OwnerSettings
{
    public const string SectionName = "Owner";

    public string Email { get; set; } = string.Empty;

    /// <summary>Formato internacional sin signos, ej. "573001234567" (requerido por la API de WhatsApp).</summary>
    public string WhatsAppNumber { get; set; } = string.Empty;
}
