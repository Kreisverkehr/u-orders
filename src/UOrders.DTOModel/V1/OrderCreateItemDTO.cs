namespace UOrders.DTOModel.V1;

public class OrderCreateItemDTO
{
    #region Public Properties

    public IEnumerable<int> CheckedOptions { get; set; } = new List<int>();

    public int Count { get; set; }

    public int MenuItemID { get; set; }

    public string? Remarks { get; set; }

    #endregion Public Properties
}
