namespace UOrders.DTOModel.V1;

public class MenuItemOptionValueDTO
{
    #region Public Properties

    public int ID { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal? PriceChangeToBase { get; set; }

    #endregion Public Properties
}
