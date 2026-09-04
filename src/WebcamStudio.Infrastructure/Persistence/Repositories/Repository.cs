using Microsoft.EntityFrameworkCore;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Domain.Common;

namespace WebcamStudio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementacion generica de IRepository&lt;T&gt; sobre EF Core. Los repositorios
/// especificos de cada modulo heredan de esta clase para no reescribir el CRUD basico
/// y solo agregan los metodos de consulta particulares que necesitan.
/// </summary>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await DbSet.FindAsync(new object[] { id }, ct);

    public virtual async Task<List<T>> GetAllAsync(CancellationToken ct = default) =>
        await DbSet.AsNoTracking().ToListAsync(ct);

    public virtual async Task AddAsync(T entity, CancellationToken ct = default) =>
        await DbSet.AddAsync(entity, ct);

    public virtual void Update(T entity) => DbSet.Update(entity);

    public virtual void Remove(T entity) => DbSet.Remove(entity);
}
