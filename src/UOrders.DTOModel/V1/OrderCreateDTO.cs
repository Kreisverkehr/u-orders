namespace UOrders.DTOModel.V1;

public class OrderCreateDTO
{
    #region Public Properties

    public string? CustomerEmail { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string? CustomerPhone { get; set; }

    public IEnumerable<OrderCreateItemDTO> Items { get; set; } = new List<OrderCreateItemDTO>();

    public string? Remarks { get; set; } = string.Empty;

    #endregion Public Properties
}
