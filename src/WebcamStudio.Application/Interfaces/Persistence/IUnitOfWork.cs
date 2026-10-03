using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Interfaces.Persistence;

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
