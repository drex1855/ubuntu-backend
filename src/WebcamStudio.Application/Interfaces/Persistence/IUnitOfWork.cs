using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Interfaces.Persistence;

/// <summary>
/// Punto unico de acceso a todos los repositorios y a SaveChangesAsync.
/// Que un modulo (ej. Checklist) toque varias tablas (ChecklistRun, ChecklistItemResult,
/// MaintenanceRequest) y las guarde en un solo SaveChangesAsync es lo que garantiza que
/// F10 ("Guardar checklist") sea atomico: o se guarda todo o no se guarda nada.
///
/// Cuando se agregue un modulo nuevo con entidades propias, el patron es: crear la
/// entidad en Domain, su repositorio (generico o especifico) aqui, y agregar la
/// propiedad correspondiente a esta interfaz + su implementacion en Infrastructure.
/// </summary>
public interface IUnitOfWork
{
    IModelAccountRepository ModelAccounts { get; }
    IContactRepository Contacts { get; }
    IRepository<Tag> Tags { get; }
    IProductRepository Products { get; }
    IRepository<Site> Sites { get; }
    ITokenReportRepository TokenReports { get; }
    IRoomRepository Rooms { get; }
    IRepository<ChecklistTemplateItem> ChecklistTemplateItems { get; }
    IChecklistRunRepository ChecklistRuns { get; }
    IRepository<ChecklistItemResult> ChecklistItemResults { get; }
    IRepository<MaintenanceRequest> MaintenanceRequests { get; }
    IRepository<AuditLog> AuditLogs { get; }
    ILoanRequestRepository LoanRequests { get; }
    IFineRepository Fines { get; }
    IStoreSaleRepository StoreSales { get; }
    IRepository<StoreDebtPayment> StoreDebtPayments { get; }
    IHourEntryRepository HourEntries { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
