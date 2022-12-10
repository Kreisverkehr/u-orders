namespace UOrders.DTOModel.V1;

public class MenuItemOptionValueDetailedDTO
{
    #region Public Properties

    public int ID { get; set; }

    public IDictionary<string, string> Name { get; set; } = new Dictionary<string, string>();

    public decimal? PriceChangeToBase { get; set; }

    #endregion Public Properties
}