using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Entities;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Infrastructure.Persistence;

/// <summary>
/// Resuelve un problema real de arranque: todos los endpoints para crear/gestionar
/// cuentas (ModelAccountsController) exigen rol Admin, pero sin ninguna cuenta existente
/// no hay forma de autenticarse para crear la primera. Este seeder crea un unico Admin
/// inicial a partir de appsettings ("Seed:AdminEmail"/"Seed:AdminPassword") la primera
/// vez que la tabla ModelAccounts esta vacia. Es idempotente: en cualquier arranque
/// posterior, ya hay al menos una cuenta, asi que no hace nada.
///
/// Por seguridad, cambiar la contraseña del admin inicial (o desactivar el seeder con
/// "Seed:Enabled": false) apenas se tenga otra cuenta Admin creada.
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(
        AppDbContext db, IPasswordHasher passwordHasher, IConfiguration configuration, ILogger logger)
    {
        var seedEnabled = configuration.GetValue<bool?>("Seed:Enabled") ?? true;
        if (!seedEnabled)
            return;

        if (await db.ModelAccounts.AnyAsync())
            return;

        var email = configuration["Seed:AdminEmail"] ?? "admin@webcamstudio.local";
        var password = configuration["Seed:AdminPassword"] ?? "ChangeMe123!";
        var fullName = configuration["Seed:AdminFullName"] ?? "Administrador Inicial";

        var admin = new ModelAccount
        {
            FullName = fullName,
            Email = email,
            Role = AccountRole.Admin,
            Status = AccountStatus.Activo,
            PasswordHash = passwordHasher.Hash(password)
        };

        db.ModelAccounts.Add(admin);
        await db.SaveChangesAsync();

        logger.LogWarning(
            "Se creo la cuenta Admin inicial ({Email}). Cambia la contraseña por defecto cuanto antes.", email);
    }
}
