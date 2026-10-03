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

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Falta la cadena de conexion 'Default' en la configuracion.");

        services.AddSingleton<SlowQueryLoggingInterceptor>();
        services.AddDbContext<AppDbContext>((sp, options) =>
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)))
                .AddInterceptors(sp.GetRequiredService<SlowQueryLoggingInterceptor>()));

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
        services.Configure<WhatsAppSettings>(configuration.GetSection(WhatsAppSettings.SectionName));
        services.Configure<OwnerSettings>(configuration.GetSection(OwnerSettings.SectionName));
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
