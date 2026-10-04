using WebcamStudio.Application.Common;

namespace WebcamStudio.Application.Interfaces.Services;

public record StoredFile(string FileName, string ContentType);


public interface IFileStorageService
{
    Task<Result<StoredFile>> SaveImageAsync(Stream content, string declaredContentType, CancellationToken ct = default);

    
    Task<(Stream Content, string ContentType)?> OpenReadAsync(string storedFileName, CancellationToken ct = default);
}
