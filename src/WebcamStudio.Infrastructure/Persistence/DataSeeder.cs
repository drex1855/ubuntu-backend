using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Entities;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Infrastructure.Persistence;

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
            "Se creo la cuenta Admin inicial ({Email}). Cambia la contraseÃ±a por defecto cuanto antes.", email);
    }
}
