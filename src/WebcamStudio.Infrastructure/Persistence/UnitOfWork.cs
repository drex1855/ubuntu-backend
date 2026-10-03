using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Domain.Entities;
using WebcamStudio.Infrastructure.Persistence.Repositories;

namespace WebcamStudio.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private IModelAccountRepository? _modelAccounts;
    private IContactRepository? _contacts;
    private IRepository<Tag>? _tags;
    private IProductRepository? _products;
    private IRepository<Site>? _sites;
    private ITokenReportRepository? _tokenReports;
    private IRoomRepository? _rooms;
    private IRepository<ChecklistTemplateItem>? _checklistTemplateItems;
    private IChecklistRunRepository? _checklistRuns;
    private IRepository<ChecklistItemResult>? _checklistItemResults;
    private IRepository<MaintenanceRequest>? _maintenanceRequests;
    private IRepository<AuditLog>? _auditLogs;
    private ILoanRequestRepository? _loanRequests;
    private IFineRepository? _fines;
    private IStoreSaleRepository? _storeSales;
    private IRepository<StoreDebtPayment>? _storeDebtPayments;
    private IHourEntryRepository? _hourEntries;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IModelAccountRepository ModelAccounts => _modelAccounts ??= new ModelAccountRepository(_context);
    public IContactRepository Contacts => _contacts ??= new ContactRepository(_context);
    public IRepository<Tag> Tags => _tags ??= new Repository<Tag>(_context);
    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public IRepository<Site> Sites => _sites ??= new Repository<Site>(_context);
    public ITokenReportRepository TokenReports => _tokenReports ??= new TokenReportRepository(_context);
    public IRoomRepository Rooms => _rooms ??= new RoomRepository(_context);
    public IRepository<ChecklistTemplateItem> ChecklistTemplateItems =>
        _checklistTemplateItems ??= new Repository<ChecklistTemplateItem>(_context);
    public IChecklistRunRepository ChecklistRuns => _checklistRuns ??= new ChecklistRunRepository(_context);
    public IRepository<ChecklistItemResult> ChecklistItemResults =>
        _checklistItemResults ??= new Repository<ChecklistItemResult>(_context);
    public IRepository<MaintenanceRequest> MaintenanceRequests =>
        _maintenanceRequests ??= new Repository<MaintenanceRequest>(_context);
    public IRepository<AuditLog> AuditLogs => _auditLogs ??= new Repository<AuditLog>(_context);
    public ILoanRequestRepository LoanRequests => _loanRequests ??= new LoanRequestRepository(_context);
    public IFineRepository Fines => _fines ??= new FineRepository(_context);
    public IStoreSaleRepository StoreSales => _storeSales ??= new StoreSaleRepository(_context);
    public IRepository<StoreDebtPayment> StoreDebtPayments =>
        _storeDebtPayments ??= new Repository<StoreDebtPayment>(_context);
    public IHourEntryRepository HourEntries => _hourEntries ??= new HourEntryRepository(_context);

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
}
