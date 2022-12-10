namespace UOrders.DTOModel.V1;

public class MenuItemUpdateDTO
{
    #region Public Properties

    public IDictionary<string, string> Description { get; set; } = new Dictionary<string, string>();

    public IDictionary<string, string> Title { get; set; } = new Dictionary<string, string>();

    #endregion Public Properties
}