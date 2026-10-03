using WebcamStudio.Application.Common;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Application.Inventory;


public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public InventoryService(IUnitOfWork unitOfWork, IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<List<ProductDto>> GetProductsAsync(CancellationToken ct = default)
    {
        var products = await _unitOfWork.Products.GetActiveAsync(ct);
        return products.Select(ToDto).ToList();
    }

    public async Task<Result<ProductDto>> CreateProductAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<ProductDto>.Failure("El nombre del producto es obligatorio.");

        if (request.Price < 0)
            return Result<ProductDto>.Failure("El precio no puede ser negativo.");

        var product = new Product
        {
            Name = request.Name.Trim(),
            Price = request.Price,
            IsActive = true
        };

        await _unitOfWork.Products.AddAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Tienda", "ProductoCreado", nameof(Product), product.Id, null, ct);

        return Result<ProductDto>.Success(ToDto(product));
    }

    public async Task<Result<ProductDto>> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<ProductDto>.Failure("El nombre del producto es obligatorio.");

        if (request.Price < 0)
            return Result<ProductDto>.Failure("El precio no puede ser negativo.");

        var product = await _unitOfWork.Products.GetByIdAsync(id, ct);
        if (product is null)
            return Result<ProductDto>.Failure("Producto no encontrado.");

        product.Name = request.Name.Trim();
        product.Price = request.Price;
        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Tienda", "ProductoActualizado", nameof(Product), product.Id,
            new { request.Name, request.Price }, ct);

        return Result<ProductDto>.Success(ToDto(product));
    }

    public async Task<Result> DeleteProductAsync(Guid id, CancellationToken ct = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, ct);
        if (product is null)
            return Result.Failure("Producto no encontrado.");

        product.IsActive = false;
        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Tienda", "ProductoEliminado", nameof(Product), product.Id, null, ct);

        return Result.Success();
    }

    public async Task<Result<StoreSaleDto>> RegisterSaleAsync(
        Guid soldByAccountId, RegisterSaleRequest request, CancellationToken ct = default)
    {
        if (request.Quantity <= 0)
            return Result<StoreSaleDto>.Failure("La cantidad debe ser mayor a cero.");

        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, ct);
        if (product is null)
            return Result<StoreSaleDto>.Failure("Producto no encontrado.");

        var account = await _unitOfWork.ModelAccounts.GetByIdAsync(request.ModelAccountId, ct);
        if (account is null)
            return Result<StoreSaleDto>.Failure("Cuenta no encontrada.");

        var sale = new StoreSale
        {
            ProductId = product.Id,
            ModelAccountId = account.Id,
            Quantity = request.Quantity,
            UnitPrice = product.Price,
            TotalAmount = product.Price * request.Quantity,
            IsCredit = request.IsCredit,
            SoldByAccountId = soldByAccountId,
            SoldAt = DateTime.UtcNow
        };
        await _unitOfWork.StoreSales.AddAsync(sale, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Tienda", "ConsumoRegistrado", nameof(StoreSale), sale.Id,
            new { request.ProductId, request.ModelAccountId, request.Quantity, sale.TotalAmount, sale.IsCredit }, ct);

        return Result<StoreSaleDto>.Success(new StoreSaleDto(
            sale.Id, product.Id, product.Name, account.Id, account.FullName,
            sale.Quantity, sale.UnitPrice, sale.TotalAmount, sale.IsCredit, sale.SoldAt));
    }

    public async Task<Result> RegisterPaymentAsync(
        Guid registeredByAccountId, RegisterDebtPaymentRequest request, CancellationToken ct = default)
    {
        if (request.Amount <= 0)
            return Result.Failure("El monto debe ser mayor a cero.");

        var account = await _unitOfWork.ModelAccounts.GetByIdAsync(request.ModelAccountId, ct);
        if (account is null)
            return Result.Failure("Cuenta no encontrada.");

        var payment = new StoreDebtPayment
        {
            ModelAccountId = account.Id,
            Amount = request.Amount,
            RegisteredByAccountId = registeredByAccountId,
            PaidAt = DateTime.UtcNow
        };
        await _unitOfWork.StoreDebtPayments.AddAsync(payment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Tienda", "AbonoDeudaRegistrado", nameof(StoreDebtPayment), payment.Id,
            new { request.ModelAccountId, request.Amount }, ct);

        return Result.Success();
    }

    public async Task<List<ModelDebtDto>> GetDebtsAsync(CancellationToken ct = default)
    {
        var sales = (await _unitOfWork.StoreSales.SearchAsync(null, ct)).Where(s => s.IsCredit);
        var payments = await _unitOfWork.StoreDebtPayments.GetAllAsync(ct);
        var paymentsByModel = payments
            .GroupBy(p => p.ModelAccountId)
            .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));

        return sales
            .GroupBy(s => new { s.ModelAccountId, s.ModelAccount.FullName })
            .Select(g => new ModelDebtDto(
                g.Key.ModelAccountId,
                g.Key.FullName,
                g.Sum(s => s.TotalAmount) - paymentsByModel.GetValueOrDefault(g.Key.ModelAccountId)))
            .OrderByDescending(d => d.TotalDebt)
            .ToList();
    }

    public async Task<ModelDebtDto> GetMyDebtAsync(Guid modelAccountId, CancellationToken ct = default)
    {
        var account = await _unitOfWork.ModelAccounts.GetByIdAsync(modelAccountId, ct);
        var sales = (await _unitOfWork.StoreSales.SearchAsync(modelAccountId, ct)).Where(s => s.IsCredit);
        var payments = await _unitOfWork.StoreDebtPayments.GetAllAsync(ct);

        var totalSales = sales.Sum(s => s.TotalAmount);
        var totalPaid = payments.Where(p => p.ModelAccountId == modelAccountId).Sum(p => p.Amount);

        return new ModelDebtDto(modelAccountId, account?.FullName ?? string.Empty, totalSales - totalPaid);
    }

    public async Task<List<StoreSaleDto>> GetSalesAsync(Guid? modelAccountId, CancellationToken ct = default)
    {
        var sales = await _unitOfWork.StoreSales.SearchAsync(modelAccountId, ct);
        return sales
            .Select(s => new StoreSaleDto(
                s.Id, s.ProductId, s.Product.Name, s.ModelAccountId, s.ModelAccount.FullName,
                s.Quantity, s.UnitPrice, s.TotalAmount, s.IsCredit, s.SoldAt))
            .ToList();
    }

    private static ProductDto ToDto(Product p) => new(p.Id, p.Name, p.Price, p.IsActive);
}
