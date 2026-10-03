using WebcamStudio.Application.Common;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Entities;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.ModelAccounts;


public class ModelAccountService : IModelAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuditService _auditService;

    public ModelAccountService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _auditService = auditService;
    }

    public async Task<List<ModelAccountDto>> GetAllAsync(CancellationToken ct = default)
    {
        
        var accounts = await _unitOfWork.ModelAccounts.GetAllAsync(ct);
        return accounts.Select(ToDto).ToList();
    }

    public async Task<Result<ModelAccountDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var account = await _unitOfWork.ModelAccounts.GetByIdAsync(id, ct);
        return account is null
            ? Result<ModelAccountDto>.Failure("Cuenta no encontrada.")
            : Result<ModelAccountDto>.Success(ToDto(account));
    }

    public async Task<Result<ModelAccountDto>> CreateAsync(CreateModelAccountRequest request, CancellationToken ct = default)
    {
        
        var existing = await _unitOfWork.ModelAccounts.GetByEmailAsync(request.Email, ct);
        if (existing is not null)
            return Result<ModelAccountDto>.Failure("Ya existe una cuenta con ese correo.");

        var account = new ModelAccount
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Role = request.Role,
            Status = AccountStatus.Activo,
            Gender = request.Gender,
            PasswordHash = _passwordHasher.Hash(request.Password)
        };

        await _unitOfWork.ModelAccounts.AddAsync(account, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Cuentas", "CuentaCreada", nameof(ModelAccount), account.Id, null, ct);

        return Result<ModelAccountDto>.Success(ToDto(account));
    }

    public async Task<Result<ModelAccountDto>> UpdateAsync(Guid id, UpdateModelAccountRequest request, CancellationToken ct = default)
    {
        
        var account = await _unitOfWork.ModelAccounts.GetByIdAsync(id, ct);
        if (account is null)
            return Result<ModelAccountDto>.Failure("Cuenta no encontrada.");

        account.FullName = request.FullName;
        account.PhoneNumber = request.PhoneNumber;
        account.Gender = request.Gender;

        _unitOfWork.ModelAccounts.Update(account);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Cuentas", "CuentaActualizada", nameof(ModelAccount), account.Id, null, ct);

        return Result<ModelAccountDto>.Success(ToDto(account));
    }

    public async Task<Result<ModelAccountDto>> SetStatusAsync(Guid id, UpdateAccountStatusRequest request, CancellationToken ct = default)
    {
        var account = await _unitOfWork.ModelAccounts.GetByIdAsync(id, ct);
        if (account is null)
            return Result<ModelAccountDto>.Failure("Cuenta no encontrada.");

   
        if (request.Status == AccountStatus.Activo)
            account.Activate();
        else
            account.Deactivate();

       
        _unitOfWork.ModelAccounts.Update(account);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Cuentas", $"EstadoCambiadoA{request.Status}", nameof(ModelAccount), account.Id, null, ct);

        return Result<ModelAccountDto>.Success(ToDto(account));
    }

    public async Task<Result> ChangePasswordAsync(Guid id, ChangePasswordRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 8)
            return Result.Failure("La nueva contraseña debe tener al menos 8 caracteres.");

        var account = await _unitOfWork.ModelAccounts.GetByIdAsync(id, ct);
        if (account is null)
            return Result.Failure("Cuenta no encontrada.");

        if (!_passwordHasher.Verify(request.CurrentPassword, account.PasswordHash))
            return Result.Failure("La contraseña actual no es correcta.");

        account.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        _unitOfWork.ModelAccounts.Update(account);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Cuentas", "ContrasenaCambiada", nameof(ModelAccount), account.Id, null, ct);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(Guid id, ResetPasswordRequest request, CancellationToken ct = default)
    {
        var account = await _unitOfWork.ModelAccounts.GetByIdAsync(id, ct);
        if (account is null)
            return Result.Failure("Cuenta no encontrada.");

        account.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        
        account.FailedLoginAttempts = 0;
        account.LockedUntil = null;

        _unitOfWork.ModelAccounts.Update(account);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Cuentas", "ContrasenaRestablecidaPorAdmin", nameof(ModelAccount), account.Id, null, ct);

        return Result.Success();
    }

    private static ModelAccountDto ToDto(ModelAccount a) =>
        new(a.Id, a.FullName, a.Email, a.PhoneNumber, a.Role, a.Status, a.Gender, a.CreatedAt);
}
