using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Interfaces.Persistence;

public interface IContactRepository : IRepository<Contact>
{
    /// <summary>Busca contactos filtrando opcionalmente por nombre/telefono, etiqueta y si tienen correo.</summary>
    Task<List<Contact>> SearchAsync(string? search, Guid? tagId, bool? hasEmail, CancellationToken ct = default);

    /// <summary>Trae un contacto con sus etiquetas cargadas (para asignar/quitar tags).</summary>
    Task<Contact?> GetByIdWithTagsAsync(Guid id, CancellationToken ct = default);
}
