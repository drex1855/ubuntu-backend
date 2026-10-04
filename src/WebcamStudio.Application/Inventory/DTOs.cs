using System.ComponentModel.DataAnnotations;

namespace WebcamStudio.Application.Inventory;

public record ProductDto(Guid Id, string Name, decimal Price, bool IsActive);

public record CreateProductRequest([Required, MaxLength(200)] string Name, [Range(0, 1_000_000)] decimal Price);

public record UpdateProductRequest([Required, MaxLength(200)] string Name, [Range(0, 1_000_000)] decimal Price);

public record RegisterSaleRequest(
    Guid ProductId, Guid ModelAccountId, [Range(1, 10_000)] int Quantity, bool IsCredit);

public record StoreSaleDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    Guid ModelAccountId,
    string ModelFullName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalAmount,
    bool IsCredit,
    DateTime SoldAt);

public record RegisterDebtPaymentRequest(Guid ModelAccountId, [Range(0.01, 1_000_000)] decimal Amount);

public record ModelDebtDto(Guid ModelAccountId, string ModelFullName, decimal TotalDebt);
