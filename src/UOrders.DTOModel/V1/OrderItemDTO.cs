namespace UOrders.DTOModel.V1;

public class OrderItemDTO
{
    public int ID { get; set; }
    public int MenuItemID { get; set; }
    public int Count { get; set; }
    public string? Remarks { get; set; }
    public string MenuItemTitle { get; set; } = string.Empty;
    public string MenuItemDescription { get; set; } = string.Empty;
    public string MenuItemCategoryTitle { get; set; } = string.Empty;
    public int MenuItemCategoryID { get; set; }
    public string MenuItemCategoryDescription { get; set; } = string.Empty;
    public IEnumerable<OrderItemOptionDTO> CheckedOptions { get; set; } = new List<OrderItemOptionDTO>();
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
