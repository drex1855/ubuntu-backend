using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.Common;

public class CurrentUser
{
    public Guid AccountId { get; set; }
    public string Email { get; set; } = string.Empty;
    public AccountRole Role { get; set; }
}
