using System.ComponentModel.DataAnnotations;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.ModelAccounts;

public record ModelAccountDto(
    Guid Id,
    string FullName,
    string Email,
    string? PhoneNumber,
    AccountRole Role,
    AccountStatus Status,
    AccountGender? Gender,
    DateTime CreatedAt);

public record CreateModelAccountRequest(
    [Required, MaxLength(200)] string FullName,
    [Required, EmailAddress, MaxLength(256)] string Email,
    [Phone, MaxLength(30)] string? PhoneNumber,
    [Required, MinLength(8), MaxLength(200)] string Password,
    AccountRole Role,
    AccountGender? Gender);

public record UpdateModelAccountRequest(
    [Required, MaxLength(200)] string FullName,
    [Phone, MaxLength(30)] string? PhoneNumber,
    AccountGender? Gender);

public record UpdateAccountStatusRequest(AccountStatus Status);

public record ChangePasswordRequest(
    [Required, MaxLength(200)] string CurrentPassword,
    [Required, MinLength(8), MaxLength(200)] string NewPassword);


public record ResetPasswordRequest([Required, MinLength(8), MaxLength(200)] string NewPassword);
