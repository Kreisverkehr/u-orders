namespace UOrders.DTOModel.V1;

public class MenuItemDetailedDTO
{
    #region Public Properties

    public decimal? CurrentPrice { get; set; }

    public IDictionary<string, string> Description { get; set; } = new Dictionary<string, string>();

    public int ID { get; set; }

    public IEnumerable<MenuItemOptionDetailedDTO> Options { get; set; } = new List<MenuItemOptionDetailedDTO>();

    public IDictionary<string, string> Title { get; set; } = new Dictionary<string, string>();

    #endregion Public Properties
}
