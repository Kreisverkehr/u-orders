namespace UOrders.DTOModel.V1;

public class MenuItemOptionDetailedDTO
{
    #region Public Properties

    public IDictionary<string, string> Description { get; set; } = new Dictionary<string, string>();

    public int ID { get; set; }

    public IDictionary<string, string> Name { get; set; } = new Dictionary<string, string>();

    public MenuItemOptionTypeDTO OptionType { get; set; }

    public IEnumerable<MenuItemOptionValueDetailedDTO> Values { get; set; } = new List<MenuItemOptionValueDetailedDTO>();

    #endregion Public Properties
}
