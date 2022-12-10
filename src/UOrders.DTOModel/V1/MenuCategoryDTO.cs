namespace UOrders.DTOModel.V1;

public class MenuCategoryDTO
{
    #region Public Properties

    public string Description { get; set; } = string.Empty;

    public int ID { get; set; }

    public ICollection<MenuItemDTO> Items { get; set; } = new List<MenuItemDTO>();

    public string Title { get; set; } = string.Empty;

    #endregion Public Properties
}