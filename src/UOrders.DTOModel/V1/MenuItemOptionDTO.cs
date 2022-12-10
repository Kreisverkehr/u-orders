namespace UOrders.DTOModel.V1;

public class MenuItemOptionDTO
{
    #region Public Properties

    public string Description { get; set; } = string.Empty;

    public int ID { get; set; }

    public string Name { get; set; } = string.Empty;

    public MenuItemOptionTypeDTO OptionType { get; set; }

    public IEnumerable<MenuItemOptionValueDTO> Values { get; set; } = new List<MenuItemOptionValueDTO>();

    #endregion Public Properties
}
