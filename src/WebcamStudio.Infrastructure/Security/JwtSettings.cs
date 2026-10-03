namespace WebcamStudio.Infrastructure.Security;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "WebcamStudio.Api";
    public string Audience { get; set; } = "WebcamStudio.Clients";
    public int ExpiryMinutes { get; set; } = 480;
}
