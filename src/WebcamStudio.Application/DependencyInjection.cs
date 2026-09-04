using Microsoft.Extensions.DependencyInjection;
using WebcamStudio.Application.Auth;
using WebcamStudio.Application.Checklists;
using WebcamStudio.Application.Contacts;
using WebcamStudio.Application.Fines;
using WebcamStudio.Application.HourEntries;
using WebcamStudio.Application.Inventory;
using WebcamStudio.Application.LoanRequests;
using WebcamStudio.Application.ModelAccounts;
using WebcamStudio.Application.Sites;
using WebcamStudio.Application.TokenReports;

namespace WebcamStudio.Application;

/// <summary>
/// Registra todos los servicios de caso de uso (uno por modulo). Cuando se agregue
/// un modulo nuevo, el patron a seguir es siempre el mismo: DTOs.cs + I&lt;Modulo&gt;Service.cs +
/// &lt;Modulo&gt;Service.cs dentro de su propia carpeta, y una linea aqui.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IModelAccountService, ModelAccountService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<ITokenReportService, TokenReportService>();
        services.AddScoped<ISiteService, SiteService>();
        services.AddScoped<IChecklistService, ChecklistService>();
        services.AddScoped<ILoanRequestService, LoanRequestService>();
        services.AddScoped<IFineService, FineService>();
        services.AddScoped<IHourEntryService, HourEntryService>();

        return services;
    }
}
