using OrderHub.Core.Domain;
using OrderHub.Core.Interfaces;

namespace OrderHub.Core.Services;

public class ProductService : IProductService
{
    private const int SalesWindowDays = 30;

    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public ProductService(IProductRepository productRepository, IOrderRepository orderRepository)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    public Task<IReadOnlyList<Product>> GetAllAsync() => _productRepository.GetAllAsync();

    public Task<IReadOnlyList<Product>> GetActiveAsync() => _productRepository.GetActiveAsync();

    public async Task<IReadOnlyList<LowStockItem>> GetLowStockAsync(int threshold)
    {
        var products = await _productRepository.GetLowStockAsync(threshold);
        if (products.Count == 0)
            return Array.Empty<LowStockItem>();

        var since = DateTime.UtcNow.AddDays(-SalesWindowDays);
        var productIds = products.Select(p => p.Id).ToList();
        var sold = await _orderRepository.GetSoldQuantitiesAsync(since, productIds);

        return products
            .Select(p => new LowStockItem(p, sold.GetValueOrDefault(p.Id)))
            .ToList();
    }
}
