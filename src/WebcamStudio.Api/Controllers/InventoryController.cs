using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebcamStudio.Application.Common;
using WebcamStudio.Application.Inventory;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Api.Controllers;


[Authorize]
public class InventoryController : ApiControllerBase
{
    private readonly IInventoryService _service;

    public InventoryController(IInventoryService service)
    {
        _service = service;
    }

    private const string StaffRoles = $"{nameof(AccountRole.Admin)},{nameof(AccountRole.Monitor)}";

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts(CancellationToken ct)
    {
        var products = await _service.GetProductsAsync(ct);
        return Ok(ApiResponse<List<ProductDto>>.Ok(products));
    }

    [Authorize(Roles = StaffRoles)]
    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken ct) =>
        HandleResult(await _service.CreateProductAsync(request, ct));

    
    [Authorize(Roles = StaffRoles)]
    [HttpPut("products/{id:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct) =>
        HandleResult(await _service.UpdateProductAsync(id, request, ct));

    
    [Authorize(Roles = StaffRoles)]
    [HttpDelete("products/{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken ct) =>
        HandleResult(await _service.DeleteProductAsync(id, ct));

    [Authorize(Roles = StaffRoles)]
    [HttpPost("sales")]
    public async Task<IActionResult> RegisterSale([FromBody] RegisterSaleRequest request, CancellationToken ct) =>
        HandleResult(await _service.RegisterSaleAsync(CurrentAccountId, request, ct));

    
    [Authorize(Roles = StaffRoles)]
    [HttpPost("payments")]
    public async Task<IActionResult> RegisterPayment([FromBody] RegisterDebtPaymentRequest request, CancellationToken ct) =>
        HandleResult(await _service.RegisterPaymentAsync(CurrentAccountId, request, ct));

    
    [Authorize(Roles = StaffRoles)]
    [HttpGet("debts")]
    public async Task<IActionResult> GetDebts(CancellationToken ct)
    {
        var debts = await _service.GetDebtsAsync(ct);
        return Ok(ApiResponse<List<ModelDebtDto>>.Ok(debts));
    }

    
    [HttpGet("debts/me")]
    public async Task<IActionResult> GetMyDebt(CancellationToken ct)
    {
        var debt = await _service.GetMyDebtAsync(CurrentAccountId, ct);
        return Ok(ApiResponse<ModelDebtDto>.Ok(debt));
    }

   
    [Authorize(Roles = StaffRoles)]
    [HttpGet("sales")]
    public async Task<IActionResult> GetSales([FromQuery] Guid? modelAccountId, CancellationToken ct)
    {
        var sales = await _service.GetSalesAsync(modelAccountId, ct);
        return Ok(ApiResponse<List<StoreSaleDto>>.Ok(sales));
    }
}
