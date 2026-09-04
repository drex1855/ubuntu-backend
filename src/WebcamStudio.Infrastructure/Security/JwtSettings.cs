namespace WebcamStudio.Infrastructure.Security;

/// <summary>
/// Se enlaza a la seccion "Jwt" de appsettings.json (ver appsettings.json en el proyecto Api).
/// El Secret NUNCA debe quedar hardcodeado ni commiteado en texto plano en un repo real:
/// en produccion se debe inyectar por variable de entorno o un secret manager
/// (Azure Key Vault, AWS Secrets Manager, etc.).
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "WebcamStudio.Api";
    public string Audience { get; set; } = "WebcamStudio.Clients";
    public int ExpiryMinutes { get; set; } = 480;
}
