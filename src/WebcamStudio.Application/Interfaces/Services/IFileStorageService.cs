using WebcamStudio.Application.Common;

namespace WebcamStudio.Application.Interfaces.Services;

public record StoredFile(string FileName, string ContentType);

/// <summary>
/// Abstrae donde y como se guardan los archivos subidos (hoy solo fotos del checklist).
/// La implementacion valida tipo/tamano/firma real del archivo antes de guardarlo con un
/// nombre aleatorio -- ver LocalFileStorageService. El resto de la app nunca toca el
/// disco directamente ni confia en el nombre/Content-Type que manda el cliente.
/// </summary>
public interface IFileStorageService
{
    Task<Result<StoredFile>> SaveImageAsync(Stream content, string declaredContentType, CancellationToken ct = default);

    /// <summary>Null si el archivo no existe (ya se borro, o el nombre no es valido).</summary>
    Task<(Stream Content, string ContentType)?> OpenReadAsync(string storedFileName, CancellationToken ct = default);
}
