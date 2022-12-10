using UOrders.DTOModel.V1;

namespace UOrders.WebUI.Model;

public class MenuItemOptionValueEdit
{
    #region Private Fields

    private decimal originalPrice;

    #endregion Private Fields

    #region Public Properties

    public bool HasPriceChanged => PriceChangeToBase != originalPrice;

    public int ID { get; set; }

    public int MenuItemOptionId { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<LocalizedText> NameLocalized { get; set; } = new List<LocalizedText>();

    public decimal PriceChangeToBase { get; set; }

    #endregion Public Properties

    #region Public Methods

    public static MenuItemOptionValueEdit FromDetailedDTO(MenuItemOptionValueDetailedDTO menuItemOptionValueDetailedDTO)
    {
        MenuItemOptionValueEdit menuItemOptionValueEdit = new();
        menuItemOptionValueEdit.ID = menuItemOptionValueDetailedDTO.ID;
        menuItemOptionValueEdit.Name = menuItemOptionValueDetailedDTO.Name[""];
        menuItemOptionValueEdit.PriceChangeToBase = menuItemOptionValueDetailedDTO.PriceChangeToBase ?? 0m;
        menuItemOptionValueEdit.originalPrice = menuItemOptionValueEdit.PriceChangeToBase;
        ((List<LocalizedText>)menuItemOptionValueEdit.NameLocalized).AddRange(menuItemOptionValueDetailedDTO.Name.Where(kv => kv.Key != string.Empty).Select(kv => new LocalizedText() { Lang = kv.Key, Text = kv.Value }));
        return menuItemOptionValueEdit;
    }

    public static implicit operator MenuItemOptionValueCreateDTO(MenuItemOptionValueEdit menuItemOptionValue) =>
        menuItemOptionValue.ToCreateDTO();

    public static implicit operator MenuItemOptionValueEdit(MenuItemOptionValueDetailedDTO dto) =>
        FromDetailedDTO(dto);

    public static implicit operator MenuItemOptionValueUpdateDTO(MenuItemOptionValueEdit menuItemOptionValue) =>
        menuItemOptionValue.ToUpdateDTO();

    public MenuItemOptionValueCreateDTO ToCreateDTO()
    {
        return new MenuItemOptionValueCreateDTO()
        {
            Name = NameLocalized.Union(new List<LocalizedText>() { new LocalizedText() { Text = Name } }).ToDictionary(t => t.Lang, t => t.Text),
            PriceChangeToBase = PriceChangeToBase
        };
    }

    public MenuItemOptionValueUpdateDTO ToUpdateDTO()
    {
        return new MenuItemOptionValueUpdateDTO()
        {
            Name = NameLocalized.Union(new List<LocalizedText>() { new LocalizedText() { Text = Name } }).ToDictionary(t => t.Lang, t => t.Text),
        };
    }

    #endregion Public Methods
}