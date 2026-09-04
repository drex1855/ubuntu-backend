using WebcamStudio.Domain.Common;

namespace WebcamStudio.Application.Interfaces.Persistence;

/// <summary>
/// Operaciones CRUD genericas que comparten todas las entidades. Los modulos que
/// necesitan consultas especiales (buscar por email, traer con detalles, etc.)
/// extienden esta interfaz con su propio repositorio especifico (ver IModelAccountRepository,
/// IConversationRepository, etc.) en vez de sobrecargar esta con metodos de un solo modulo.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<T>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);
}
