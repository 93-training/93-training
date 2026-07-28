using OrderHub.Core.Domain;

namespace OrderHub.Core.Services;

/// <summary>
/// 低庫存查詢結果：某項商品加上其近 30 天銷量（排除已取消訂單）。
/// </summary>
public record LowStockItem(Product Product, int SoldLast30Days);
