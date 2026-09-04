using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using WebcamStudio.Application.Common;
using WebcamStudio.Application.Interfaces.Services;

namespace WebcamStudio.Infrastructure.Services;

/// <summary>
/// Guarda archivos en disco, fuera de wwwroot (nunca se sirven como archivos estaticos --
/// solo a traves de un endpoint autenticado que llama a OpenReadAsync). Pensado para
/// fotos del checklist de habitaciones; si mas adelante otro modulo necesita subir
/// archivos, puede reusar esta misma implementacion sin cambios.
///
/// Medidas de seguridad:
/// - Nunca confia en el Content-Type ni el nombre que manda el cliente: valida los
///   primeros bytes del archivo (firma real de JPEG/PNG/WEBP) antes de aceptarlo.
/// - Limite de tamano (5 MB) validado aqui, no solo en el input del navegador.
/// - El archivo se guarda con un nombre aleatorio (GUID + extension), nunca con el
///   nombre original -- evita colisiones y cualquier intento de path traversal al guardar.
/// - Al leer, valida que el nombre pedido tenga exactamente el formato esperado
///   (GUID + extension conocida) antes de combinar la ruta, para no confiar en nada que
///   venga de la base de datos o de la URL sin revisar.
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private const long MaxBytes = 5 * 1024 * 1024;
    private static readonly Regex ValidFileNamePattern = new("^[0-9a-f]{32}\\.(jpg|png|webp)$", RegexOptions.Compiled);

    private readonly string _uploadsDirectory;

    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _uploadsDirectory = Path.Combine(environment.ContentRootPath, "App_Data", "uploads", "checklists");
        Directory.CreateDirectory(_uploadsDirectory);
    }

    public async Task<Result<StoredFile>> SaveImageAsync(Stream content, string declaredContentType, CancellationToken ct = default)
    {
        using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, ct);

        if (buffer.Length == 0)
            return Result<StoredFile>.Failure("El archivo esta vacio.");
        if (buffer.Length > MaxBytes)
            return Result<StoredFile>.Failure("La imagen no puede pesar mas de 5 MB.");

        var bytes = buffer.ToArray();
        var detected = DetectImageType(bytes);
        if (detected is null)
            return Result<StoredFile>.Failure("El archivo debe ser una imagen JPEG, PNG o WEBP valida.");

        var fileName = $"{Guid.NewGuid():N}{detected.Value.Extension}";
        var path = Path.Combine(_uploadsDirectory, fileName);
        await File.WriteAllBytesAsync(path, bytes, ct);

        return Result<StoredFile>.Success(new StoredFile(fileName, detected.Value.ContentType));
    }

    public Task<(Stream Content, string ContentType)?> OpenReadAsync(string storedFileName, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(storedFileName) || !ValidFileNamePattern.IsMatch(storedFileName))
            return Task.FromResult<(Stream, string)?>(null);

        var path = Path.Combine(_uploadsDirectory, storedFileName);
        if (!File.Exists(path))
            return Task.FromResult<(Stream, string)?>(null);

        var contentType = Path.GetExtension(storedFileName) switch
        {
            ".jpg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };

        Stream stream = File.OpenRead(path);
        return Task.FromResult<(Stream, string)?>((stream, contentType));
    }

    private static (string Extension, string ContentType)? DetectImageType(byte[] bytes)
    {
        if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            return (".jpg", "image/jpeg");

        if (bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47
            && bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A)
            return (".png", "image/png");

        if (bytes.Length >= 12
            && bytes[0] == 'R' && bytes[1] == 'I' && bytes[2] == 'F' && bytes[3] == 'F'
            && bytes[8] == 'W' && bytes[9] == 'E' && bytes[10] == 'B' && bytes[11] == 'P')
            return (".webp", "image/webp");

        return null;
    }
}
