namespace UOrders.DTOModel.V1;

public class MenuItemOptionCreateDTO
{
    #region Public Properties

    public IDictionary<string, string> Description { get; set; } = new Dictionary<string, string>();

    public IDictionary<string, string> Name { get; set; } = new Dictionary<string, string>();

    public MenuItemOptionTypeDTO OptionType { get; set; }

    public IEnumerable<MenuItemOptionValueCreateDTO> Values { get; set; } = new List<MenuItemOptionValueCreateDTO>();

    #endregion Public Properties
}
