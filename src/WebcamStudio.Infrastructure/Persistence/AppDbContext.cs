using Microsoft.EntityFrameworkCore;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Common;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence;

/// <summary>
/// DbContext principal. Todas las tablas del sistema (los 5 modulos + auditoria) viven
/// en el mismo contexto por ahora, lo cual es correcto para un modular monolith: los
/// modulos estan separados por carpeta/namespace en el codigo, pero comparten base de
/// datos para poder hacer joins simples (ej. reportes de tokens por modelo) y una sola
/// transaccion por request. Si el sistema crece tanto que un modulo necesita su propia
/// base de datos o escalar aparte, se puede partir en otro DbContext mas adelante.
/// </summary>
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
        // Aplica todas las clases IEntityTypeConfiguration<T> definidas en este ensamblado
        // (carpeta Persistence/Configurations). Un modulo nuevo solo tiene que agregar su
        // configuracion ahi y queda incluido automaticamente, sin tocar este archivo.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Llena CreatedAt/UpdatedAt/CreatedByAccountId/UpdatedByAccountId automaticamente
    /// para cualquier entidad que herede de AuditableEntity, sin que cada servicio de
    /// Application tenga que acordarse de setearlos a mano en cada Create/Update.
    /// </summary>
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
