using OrderHub.Core.Domain;

namespace OrderHub.Tests;

public class ProductServiceLowStockTests
{
    [Fact]
    public async Task GetLowStock_FiltersByThresholdAndSortsByStockAscending()
    {
        using var db = TestSetup.CreateContext();
        var service = TestSetup.CreateProductService(db);
        TestSetup.AddProduct(db, stock: 8, sku: "SKU-B008");
        TestSetup.AddProduct(db, stock: 3, sku: "SKU-B003");
        TestSetup.AddProduct(db, stock: 12, sku: "SKU-B012");

        var result = await service.GetLowStockAsync(10);

        Assert.Equal(new[] { 3, 8 }, result.Select(r => r.Product.StockQuantity));
    }

    [Fact]
    public async Task GetLowStock_ExcludesInactiveProducts()
    {
        using var db = TestSetup.CreateContext();
        var service = TestSetup.CreateProductService(db);
        TestSetup.AddProduct(db, stock: 3, sku: "SKU-ACTIVE");
        TestSetup.AddProduct(db, stock: 2, isActive: false, sku: "SKU-STOPPED");

        var result = await service.GetLowStockAsync(10);

        Assert.Single(result);
        Assert.Equal("SKU-ACTIVE", result[0].Product.Sku);
    }

    [Fact]
    public async Task GetLowStock_CountsSoldLast30DaysExcludingCancelled()
    {
        using var db = TestSetup.CreateContext();
        var service = TestSetup.CreateProductService(db);
        var customer = TestSetup.AddCustomer(db);
        var product = TestSetup.AddProduct(db, stock: 3, sku: "SKU-SALES");

        db.Orders.AddRange(
            // 近 30 天內、非取消 → 應計入
            new Order
            {
                CustomerId = customer.Id,
                Status = OrderStatus.Confirmed,
                CreatedAt = DateTime.UtcNow,
                Items = { new OrderItem { ProductId = product.Id, Quantity = 5, UnitPriceSnapshot = 100m } }
            },
            // 近 30 天內、已取消 → 應排除
            new Order
            {
                CustomerId = customer.Id,
                Status = OrderStatus.Cancelled,
                CreatedAt = DateTime.UtcNow,
                Items = { new OrderItem { ProductId = product.Id, Quantity = 7, UnitPriceSnapshot = 100m } }
            },
            // 超過 30 天 → 應排除
            new Order
            {
                CustomerId = customer.Id,
                Status = OrderStatus.Shipped,
                CreatedAt = DateTime.UtcNow.AddDays(-40),
                Items = { new OrderItem { ProductId = product.Id, Quantity = 4, UnitPriceSnapshot = 100m } }
            });
        db.SaveChanges();

        var result = await service.GetLowStockAsync(10);

        Assert.Single(result);
        Assert.Equal(5, result[0].SoldLast30Days);
    }
}
