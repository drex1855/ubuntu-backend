using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WebcamStudio.Application.Common;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Infrastructure.ExternalServices;
using WebcamStudio.Infrastructure.Persistence;
using WebcamStudio.Infrastructure.Security;
using WebcamStudio.Infrastructure.Services;

namespace WebcamStudio.Infrastructure;

/// <summary>
/// Composition root de todo lo relacionado a infraestructura: base de datos, seguridad,
/// auditoria y servicios externos. Program.cs (en el proyecto Api) solo llama a
/// AddInfrastructure() + AddApplication() y ya tiene todo el sistema conectado.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Falta la cadena de conexion 'Default' en la configuracion.");

        // Se fija la version del servidor en vez de usar ServerVersion.AutoDetect(...)
        // porque AutoDetect abre una conexion a MySQL apenas arranca la app para
        // preguntarle la version -- si el contenedor de MySQL todavia no esta listo
        // (muy comun en docker-compose al levantar todo junto), la API no arranca.
        // Con la version fija, la app arranca igual y el primer error real de conexion
        // aparece recien en el primer request que toque la base de datos.
        // Ajustar el numero de version si se usa una version de MySQL/MariaDB distinta.
        services.AddSingleton<SlowQueryLoggingInterceptor>();
        services.AddDbContext<AppDbContext>((sp, options) =>
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)))
                .AddInterceptors(sp.GetRequiredService<SlowQueryLoggingInterceptor>()));

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
        services.Configure<WhatsAppSettings>(configuration.GetSection(WhatsAppSettings.SectionName));
        services.Configure<OwnerSettings>(configuration.GetSection(OwnerSettings.SectionName));
        // LoanRequestService (Application) recibe OwnerSettings "pelado", sin envolver en
        // IOptions<T>, porque esa capa a proposito no referencia Microsoft.Extensions.Options
        // (ver comentario en WebcamStudio.Application.csproj).
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<OwnerSettings>>().Value);

        services.AddHttpContextAccessor();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAuditService, AuditService>();

        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddHttpClient<IWhatsAppNotifier, WhatsAppCloudApiNotifier>();

        return services;
    }
}
