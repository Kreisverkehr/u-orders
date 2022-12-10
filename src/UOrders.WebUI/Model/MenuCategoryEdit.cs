using UOrders.DTOModel.V1;

namespace UOrders.WebUI.Model;

public class MenuCategoryEdit
{
    #region Public Properties

    public string Description { get; set; } = string.Empty;

    public ICollection<LocalizedText> DescriptionLocalized { get; set; } = new List<LocalizedText>();

    public int ID { get; set; }

    public string Title { get; set; } = string.Empty;

    public ICollection<LocalizedText> TitleLocalized { get; set; } = new List<LocalizedText>();

    #endregion Public Properties

    #region Public Methods

    public static MenuCategoryEdit FromDetailedDTO(MenuCategoryDetailedDTO menuCategoryDetailedDTO)
    {
        MenuCategoryEdit menuCategoryEdit = new MenuCategoryEdit();
        menuCategoryEdit.ID = menuCategoryDetailedDTO.ID;
        menuCategoryEdit.Description = menuCategoryDetailedDTO.Description[""];
        menuCategoryEdit.Title = menuCategoryDetailedDTO.Title[""];
        ((List<LocalizedText>)menuCategoryEdit.DescriptionLocalized).AddRange(menuCategoryDetailedDTO.Description.Where(kv => kv.Key != string.Empty).Select(kv => new LocalizedText() { Lang = kv.Key, Text = kv.Value }));
        ((List<LocalizedText>)menuCategoryEdit.TitleLocalized).AddRange(menuCategoryDetailedDTO.Title.Where(kv => kv.Key != string.Empty).Select(kv => new LocalizedText() { Lang = kv.Key, Text = kv.Value }));
        return menuCategoryEdit;
    }

    public static implicit operator MenuCategoryCreateDTO(MenuCategoryEdit menuCategory) =>
        menuCategory.ToCreateDTO();

    public static implicit operator MenuCategoryUpdateDTO(MenuCategoryEdit menuCategory) =>
        menuCategory.ToUpdateDTO();

    public static implicit operator MenuCategoryEdit(MenuCategoryDetailedDTO dto) =>
        FromDetailedDTO(dto);

    public MenuCategoryCreateDTO ToCreateDTO()
    {
        return new MenuCategoryCreateDTO()
        {
            Title = TitleLocalized.Union(new List<LocalizedText>() { new LocalizedText() { Text = Title } }).ToDictionary(t => t.Lang, t => t.Text),
            Description = DescriptionLocalized.Union(new List<LocalizedText>() { new LocalizedText() { Text = Description } }).ToDictionary(t => t.Lang, t => t.Text),
        };
    }

    public MenuCategoryUpdateDTO ToUpdateDTO()
    {
        return new MenuCategoryUpdateDTO()
        {
            Title = TitleLocalized.Union(new List<LocalizedText>() { new LocalizedText() { Text = Title } }).ToDictionary(t => t.Lang, t => t.Text),
            Description = DescriptionLocalized.Union(new List<LocalizedText>() { new LocalizedText() { Text = Description } }).ToDictionary(t => t.Lang, t => t.Text),
        };
    }

    #endregion Public Methods
}