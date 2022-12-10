namespace UOrders.DTOModel.V1;

public class OrderItemOptionDTO
{
    public int OptionValueID { get; set; }
    public string OptionValueName { get; set; } = string.Empty;
    public string OptionValueMenuItemOptionName { get; set; } = string.Empty;
    public int OptionValueMenuItemOptionID { get; set; }
}