namespace WebcamStudio.Domain.Common;

/// <summary>
/// Clase base para todas las entidades del dominio.
/// Usamos Guid como llave primaria (en vez de int autoincremental) porque el sistema
/// esta pensado para crecer: con Guid evitamos colisiones si en el futuro se separan
/// modulos en servicios distintos, se sincronizan datos entre entornos, o se generan
/// IDs desde el cliente antes de persistir.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}

/// <summary>
/// Entidad con seguimiento de auditoria basica (quien y cuando la creo/modifico).
/// AppDbContext llena estos campos automaticamente en SaveChangesAsync, asi que
/// los servicios de aplicacion no tienen que preocuparse por setearlos a mano.
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedByAccountId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedByAccountId { get; set; }
}
