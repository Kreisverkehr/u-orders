using UOrders.DTOModel.V1;

namespace UOrders.WebUI.Model;

public class MenuItemOptionEdit
{
    #region Public Properties

    public List<MenuItemOptionValueEdit> AddedValues { get; set; } = new List<MenuItemOptionValueEdit>();

    public List<MenuItemOptionValueEdit> DeletedValues { get; set; } = new List<MenuItemOptionValueEdit>();

    public string Description { get; set; } = string.Empty;

    public ICollection<LocalizedText> DescriptionLocalized { get; set; } = new List<LocalizedText>();

    public int ID { get; set; }

    public int MenuItemId { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<LocalizedText> NameLocalized { get; set; } = new List<LocalizedText>();

    public MenuItemOptionTypeDTO OptionType { get; set; }

    public ICollection<MenuItemOptionValueEdit> Values { get; set; } = new List<MenuItemOptionValueEdit>();

    #endregion Public Properties

    #region Public Methods

    public static MenuItemOptionEdit FromDetailedDTO(MenuItemOptionDetailedDTO menuItemOptionDetailedDTO)
    {
        MenuItemOptionEdit menuItemOptionEdit = new();
        menuItemOptionEdit.ID = menuItemOptionDetailedDTO.ID;
        menuItemOptionEdit.Description = menuItemOptionDetailedDTO.Description[""];
        menuItemOptionEdit.Name = menuItemOptionDetailedDTO.Name[""];
        menuItemOptionEdit.OptionType = menuItemOptionDetailedDTO.OptionType;
        menuItemOptionEdit.Values = menuItemOptionDetailedDTO.Values.Select(x => (MenuItemOptionValueEdit)x).ToList();
        ((List<LocalizedText>)menuItemOptionEdit.DescriptionLocalized).AddRange(menuItemOptionDetailedDTO.Description.Where(kv => kv.Key != string.Empty).Select(kv => new LocalizedText() { Lang = kv.Key, Text = kv.Value }));
        ((List<LocalizedText>)menuItemOptionEdit.NameLocalized).AddRange(menuItemOptionDetailedDTO.Name.Where(kv => kv.Key != string.Empty).Select(kv => new LocalizedText() { Lang = kv.Key, Text = kv.Value }));
        return menuItemOptionEdit;
    }

    public static implicit operator MenuItemOptionCreateDTO(MenuItemOptionEdit menuCategory) =>
        menuCategory.ToCreateDTO();

    public static implicit operator MenuItemOptionEdit(MenuItemOptionDetailedDTO dto) =>
        FromDetailedDTO(dto);

    public static implicit operator MenuItemOptionUpdateDTO(MenuItemOptionEdit menuCategory) =>
        menuCategory.ToUpdateDTO();

    public MenuItemOptionValueEdit AddValue()
    {
        MenuItemOptionValueEdit menuItemOptionValueEdit = new();
        AddedValues.Add(menuItemOptionValueEdit);
        Values.Add(menuItemOptionValueEdit);
        return menuItemOptionValueEdit;
    }

    public void DeleteValue(MenuItemOptionValueEdit val)
    {
        if (AddedValues.Contains(val))
            AddedValues.Remove(val);
        else
            DeletedValues.Add(val);

        Values.Remove(val);
    }

    public MenuItemOptionCreateDTO ToCreateDTO()
    {
        return new MenuItemOptionCreateDTO()
        {
            Name = NameLocalized.Union(new List<LocalizedText>() { new LocalizedText() { Text = Name } }).ToDictionary(t => t.Lang, t => t.Text),
            Description = DescriptionLocalized.Union(new List<LocalizedText>() { new LocalizedText() { Text = Description } }).ToDictionary(t => t.Lang, t => t.Text),
            OptionType = OptionType,
            Values = Values.Select(x => (MenuItemOptionValueCreateDTO)x)
        };
    }

    public MenuItemOptionUpdateDTO ToUpdateDTO()
    {
        return new MenuItemOptionUpdateDTO()
        {
            Name = NameLocalized.Union(new List<LocalizedText>() { new LocalizedText() { Text = Name } }).ToDictionary(t => t.Lang, t => t.Text),
            Description = DescriptionLocalized.Union(new List<LocalizedText>() { new LocalizedText() { Text = Description } }).ToDictionary(t => t.Lang, t => t.Text),
            OptionType = OptionType
        };
    }

    #endregion Public Methods
}