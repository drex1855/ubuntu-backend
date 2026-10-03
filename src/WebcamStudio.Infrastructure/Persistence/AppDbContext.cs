using Microsoft.EntityFrameworkCore;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Common;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService? currentUserService = null)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<ModelAccount> ModelAccounts => Set<ModelAccount>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<TokenReport> TokenReports => Set<TokenReport>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<ChecklistTemplateItem> ChecklistTemplateItems => Set<ChecklistTemplateItem>();
    public DbSet<ChecklistRun> ChecklistRuns => Set<ChecklistRun>();
    public DbSet<ChecklistItemResult> ChecklistItemResults => Set<ChecklistItemResult>();
    public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<LoanRequest> LoanRequests => Set<LoanRequest>();
    public DbSet<Fine> Fines => Set<Fine>();
    public DbSet<StoreSale> StoreSales => Set<StoreSale>();
    public DbSet<StoreDebtPayment> StoreDebtPayments => Set<StoreDebtPayment>();
    public DbSet<HourEntry> HourEntries => Set<HourEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInfo()
    {
        var now = DateTime.UtcNow;
        var currentAccountId = _currentUserService?.AccountId;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedByAccountId = currentAccountId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedByAccountId = currentAccountId;
            }
        }
    }
}
