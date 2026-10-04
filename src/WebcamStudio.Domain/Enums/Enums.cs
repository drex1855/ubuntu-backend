namespace WebcamStudio.Domain.Enums;

public enum AccountRole
{
    Admin = 1,
    Modelo = 2,
    Monitor = 3
}

public enum AccountGender
{
    Femenino = 1,
    Masculino = 2,
    Otro = 3
}

public enum AccountStatus
{
    Activo = 1,
    Desactivado = 2
}

public enum ChecklistItemStatus
{
    Bueno = 1,
    Malo = 2
}

public enum MaintenanceRequestStatus
{
    Pendiente = 1,
    EnProceso = 2,
    Resuelta = 3
}

public enum LoanRequestStatus
{
    Pendiente = 1,
    Aprobada = 2,
    Rechazada = 3
}

public enum FineStatus
{
    PendientePorCobrar = 1,
    Pagada = 2,
    Cancelada = 3
}
