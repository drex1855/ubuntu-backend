using WebcamStudio.Application.Common;

namespace WebcamStudio.Application.ModelAccounts;

public interface IModelAccountService
{
    Task<List<ModelAccountDto>> GetAllAsync(CancellationToken ct = default);
    Task<Result<ModelAccountDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<ModelAccountDto>> CreateAsync(CreateModelAccountRequest request, CancellationToken ct = default);
    Task<Result<ModelAccountDto>> UpdateAsync(Guid id, UpdateModelAccountRequest request, CancellationToken ct = default);
    Task<Result<ModelAccountDto>> SetStatusAsync(Guid id, UpdateAccountStatusRequest request, CancellationToken ct = default);
    Task<Result> ChangePasswordAsync(Guid id, ChangePasswordRequest request, CancellationToken ct = default);

    /// <summary>Restablece la contraseña de cualquier cuenta -- solo para uso de Admin,
    /// no valida contraseña actual (ver ChangePasswordAsync para el autoservicio).</summary>
    Task<Result> ResetPasswordAsync(Guid id, ResetPasswordRequest request, CancellationToken ct = default);
}
