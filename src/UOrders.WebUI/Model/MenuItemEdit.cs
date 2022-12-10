using UOrders.DTOModel.V1;

namespace UOrders.WebUI.Model;

public class MenuItemEdit
{
    #region Private Fields

    private decimal originalPrice;

    #endregion Private Fields

    #region Public Properties

    public List<MenuItemOptionEdit> AddedOptions { get; set; } = new();

    public int CategoryId { get; set; }

    public decimal CurrentPrice { get; set; }

    public List<MenuItemOptionEdit> DeletedOptions { get; set; } = new();

    public string Description { get; set; } = string.Empty;

    public ICollection<LocalizedText> DescriptionLocalized { get; set; } = new List<LocalizedText>();

    public bool HasPriceChanged => CurrentPrice != originalPrice;

    public int ID { get; set; }

    public ICollection<MenuItemOptionEdit> Options { get; set; } = new List<MenuItemOptionEdit>();

    public string Title { get; set; } = string.Empty;

    public ICollection<LocalizedText> TitleLocalized { get; set; } = new List<LocalizedText>();

    #endregion Public Properties

    #region Public Methods

    public static MenuItemEdit FromDetailedDTO(MenuItemDetailedDTO menuItemDetailedDTO)
    {
        MenuItemEdit menuItemEdit = new();
        menuItemEdit.ID = menuItemDetailedDTO.ID;
        menuItemEdit.Description = menuItemDetailedDTO.Description[""];
        menuItemEdit.Title = menuItemDetailedDTO.Title[""];
        menuItemEdit.CurrentPrice = menuItemDetailedDTO.CurrentPrice ?? 0m;
        menuItemEdit.originalPrice = menuItemEdit.CurrentPrice;
        menuItemEdit.Options = menuItemDetailedDTO.Options.Select(x => (MenuItemOptionEdit)x).ToList();
        ((List<LocalizedText>)menuItemEdit.DescriptionLocalized).AddRange(menuItemDetailedDTO.Description.Where(kv => kv.Key != string.Empty).Select(kv => new LocalizedText() { Lang = kv.Key, Text = kv.Value }));
        ((List<LocalizedText>)menuItemEdit.TitleLocalized).AddRange(menuItemDetailedDTO.Title.Where(kv => kv.Key != string.Empty).Select(kv => new LocalizedText() { Lang = kv.Key, Text = kv.Value }));
        return menuItemEdit;
    }

    public static implicit operator MenuItemCreateDTO(MenuItemEdit menuItem) =>
        menuItem.ToCreateDTO();

    public static implicit operator MenuItemEdit(MenuItemDetailedDTO dto) =>
        FromDetailedDTO(dto);

    public static implicit operator MenuItemUpdateDTO(MenuItemEdit menuItem) =>
        menuItem.ToUpdateDTO();

    public MenuItemOptionEdit AddOption()
    {
        MenuItemOptionEdit option = new();
        AddedOptions.Add(option);
        Options.Add(option);
        return option;
    }

    public void AddOptionsFrom(IEnumerable<MenuItemOptionDetailedDTO> menuItemOptions)
    {
        foreach (var opt in menuItemOptions)
        {
            AddedOptions.Add((MenuItemOptionEdit)opt);
            Options.Add((MenuItemOptionEdit)opt);
        }
    }

    public void MarkOptionForDeletion(MenuItemOptionEdit option)
    {
        Options.Remove(option);

        if (AddedOptions.Contains(option))
            AddedOptions.Remove(option);
        else
            DeletedOptions.Add(option);
    }

    public MenuItemCreateDTO ToCreateDTO()
    {
        return new MenuItemCreateDTO()
        {
            Title = TitleLocalized.Union(new List<LocalizedText>() { new LocalizedText() { Text = Title } }).ToDictionary(t => t.Lang, t => t.Text),
            Description = DescriptionLocalized.Union(new List<LocalizedText>() { new LocalizedText() { Text = Description } }).ToDictionary(t => t.Lang, t => t.Text),
            Price = CurrentPrice,
            Options = Options.Select(x => (MenuItemOptionCreateDTO)x)
        };
    }

    public MenuItemUpdateDTO ToUpdateDTO()
    {
        return new MenuItemUpdateDTO()
        {
            Title = TitleLocalized.Union(new List<LocalizedText>() { new LocalizedText() { Text = Title } }).ToDictionary(t => t.Lang, t => t.Text),
            Description = DescriptionLocalized.Union(new List<LocalizedText>() { new LocalizedText() { Text = Description } }).ToDictionary(t => t.Lang, t => t.Text),
        };
    }

    #endregion Public Methods
}