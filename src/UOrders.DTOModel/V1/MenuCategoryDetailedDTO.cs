namespace UOrders.DTOModel.V1;

public class MenuCategoryDetailedDTO
{
    #region Public Properties

    public IDictionary<string, string> Description { get; set; } = new Dictionary<string, string>();

    public int ID { get; set; }

    public ICollection<MenuItemDetailedDTO> Items { get; set; } = new List<MenuItemDetailedDTO>();

    public IDictionary<string, string> Title { get; set; } = new Dictionary<string, string>();

    #endregion Public Properties
}