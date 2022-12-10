namespace UOrders.DTOModel.V1;

public class MenuItemDTO : IEquatable<MenuItemDTO>
{
    #region Public Properties

    public decimal? CurrentPrice { get; set; }

    public string Description { get; set; } = string.Empty;

    public int ID { get; set; }

    public IEnumerable<MenuItemOptionDTO> Options { get; set; } = new List<MenuItemOptionDTO>();

    public string Title { get; set; } = string.Empty;

    #endregion Public Properties

    #region Public Methods

    public bool Equals(MenuItemDTO? other) =>
        other is not null &&
        other.ID == ID;

    public override bool Equals(object? obj) =>
        obj is not null && obj is MenuItemDTO menuItem ? Equals(menuItem) : false;

    public override int GetHashCode() =>
        ID.GetHashCode();

    #endregion Public Methods
}