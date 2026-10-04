using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;


public class Contact : AuditableEntity
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Notes { get; set; }

    
    public string Source { get; set; } = "SitioWeb";

    public bool ConsentGiven { get; set; }
    public DateTime ConsentGivenAt { get; set; }

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
