namespace UOrders.DTOModel.V1;

public class MenuCategoryCreateDTO
{
    #region Public Properties

    public IDictionary<string, string> Description { get; set; } = new Dictionary<string, string>();

    public ICollection<MenuItemCreateDTO> Items { get; set; } = new List<MenuItemCreateDTO>();

    public IDictionary<string, string> Title { get; set; } = new Dictionary<string, string>();

    #endregion Public Properties
}
