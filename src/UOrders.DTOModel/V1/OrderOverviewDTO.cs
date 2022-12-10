namespace UOrders.DTOModel.V1;

public class OrderOverviewDTO
{
    public int ID { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; } = string.Empty;
    public DateTimeOffset OrderedOn { get; set; }
    public DateTimeOffset OrderedOnLocal => OrderedOn.ToLocalTime();
    public string? Remarks { get; set; }
    public int? OrderedItems { get; set; }
    public decimal? TotalPrice { get; set; }
}
