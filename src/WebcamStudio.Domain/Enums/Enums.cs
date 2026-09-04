namespace WebcamStudio.Domain.Enums;

/// <summary>
/// Rol de la cuenta dentro del sistema. Se mantiene como enum simple por ahora;
/// si mas adelante se necesitan permisos mas finos (ej. supervisor, contador),
/// esto se puede migrar a una tabla Roles/Permissions sin romper el resto del dominio,
/// porque el rol solo se usa para autorizar endpoints (ver Program.cs y [Authorize]).
/// </summary>
public enum AccountRole
{
    Admin = 1,
    Modelo = 2,
    Monitor = 3
}

/// <summary>
/// Genero de la cuenta de modelo. Es opcional (nullable en la entidad): no todas las
/// cuentas necesitan cargarlo (ej. cuentas Admin).
/// </summary>
public enum AccountGender
{
    Femenino = 1,
    Masculino = 2,
    Otro = 3
}

/// <summary>
/// Estado de una cuenta de modelo. Corresponde a la decision "Estado de cuenta"
/// (activo/desactivado) del modulo de Cuentas de Modelos en el diagrama.
/// </summary>
public enum AccountStatus
{
    Activo = 1,
    Desactivado = 2
}

/// <summary>
/// Resultado de revisar un elemento del checklist de habitacion.
/// Corresponde a la decision "Elemento en buen estado" del diagrama.
/// </summary>
public enum ChecklistItemStatus
{
    Bueno = 1,
    Malo = 2
}

/// <summary>
/// Estado de una solicitud de mantenimiento generada a partir de un checklist.
/// </summary>
public enum MaintenanceRequestStatus
{
    Pendiente = 1,
    EnProceso = 2,
    Resuelta = 3
}

/// <summary>
/// Estado de una solicitud de prestamo hecha por una modelo/monitor. El Admin/dueno
/// decide si se aprueba o se rechaza (ver LoanRequestsController).
/// </summary>
public enum LoanRequestStatus
{
    Pendiente = 1,
    Aprobada = 2,
    Rechazada = 3
}

/// <summary>
/// Estado de una multa aplicada a una cuenta de modelo (ver FineService).
/// </summary>
public enum FineStatus
{
    PendientePorCobrar = 1,
    Pagada = 2,
    Cancelada = 3
}
