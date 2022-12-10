namespace UOrders.DTOModel.V1;

public class MenuItemOptionUpdateDTO
{
    #region Public Properties

    public IDictionary<string, string> Description { get; set; } = new Dictionary<string, string>();

    public IDictionary<string, string> Name { get; set; } = new Dictionary<string, string>();

    public MenuItemOptionTypeDTO OptionType { get; set; }

    #endregion Public Properties
}