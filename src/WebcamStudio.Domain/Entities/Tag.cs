using WebcamStudio.Domain.Common;

namespace WebcamStudio.Domain.Entities;

public class Tag : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
