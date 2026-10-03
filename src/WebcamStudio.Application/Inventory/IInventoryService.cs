using WebcamStudio.Application.Common;

namespace WebcamStudio.Application.Inventory;

public interface IInventoryService
{
    Task<List<ProductDto>> GetProductsAsync(CancellationToken ct = default);
    Task<Result<ProductDto>> CreateProductAsync(CreateProductRequest request, CancellationToken ct = default);
    Task<Result<ProductDto>> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default);
    Task<Result> DeleteProductAsync(Guid id, CancellationToken ct = default);

    Task<Result<StoreSaleDto>> RegisterSaleAsync(
        Guid soldByAccountId, RegisterSaleRequest request, CancellationToken ct = default);
    Task<Result> RegisterPaymentAsync(
        Guid registeredByAccountId, RegisterDebtPaymentRequest request, CancellationToken ct = default);
    Task<List<ModelDebtDto>> GetDebtsAsync(CancellationToken ct = default);
    Task<ModelDebtDto> GetMyDebtAsync(Guid modelAccountId, CancellationToken ct = default);
    Task<List<StoreSaleDto>> GetSalesAsync(Guid? modelAccountId, CancellationToken ct = default);
}
