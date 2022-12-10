namespace UOrders.DTOModel.V1;

public class MenuItemOptionValueCreateDTO
{
    #region Public Properties

    public IDictionary<string, string> Name { get; set; } = new Dictionary<string, string>();

    public decimal? PriceChangeToBase { get; set; }

    #endregion Public Properties
}
