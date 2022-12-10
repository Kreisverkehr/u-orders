namespace UOrders.DTOModel.V1;

public class MenuItemCreateDTO
{
    #region Public Properties

    public IDictionary<string, string> Description { get; set; } = new Dictionary<string, string>();

    public IEnumerable<MenuItemOptionCreateDTO> Options { get; set; } = new List<MenuItemOptionCreateDTO>();

    public decimal? Price { get; set; }

    public IDictionary<string, string> Title { get; set; } = new Dictionary<string, string>();

    #endregion Public Properties
}