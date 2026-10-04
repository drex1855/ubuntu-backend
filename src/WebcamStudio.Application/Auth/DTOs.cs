using System.ComponentModel.DataAnnotations;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.Auth;

public record LoginRequest(
    [Required, EmailAddress, MaxLength(256)] string Email,
    [Required, MaxLength(200)] string Password);

public record LoginResponse(string Token, DateTime ExpiresAt, Guid AccountId, string FullName, AccountRole Role);
