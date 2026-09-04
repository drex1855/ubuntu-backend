namespace WebcamStudio.Infrastructure.ExternalServices;

/// <summary>
/// Se enlaza a la seccion "Email" de appsettings.json. Credenciales SMTP reales:
/// NUNCA deben quedar hardcodeadas ni commiteadas en texto plano en un repo real -- en
/// produccion se inyectan por variable de entorno o un secret manager (mismo criterio
/// que Jwt:Secret, ver JwtSettings.cs).
/// </summary>
public class EmailSettings
{
    public const string SectionName = "Email";

    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = "Webcam Studio";
}
