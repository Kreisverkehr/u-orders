using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UOrders.EFModel;

public class MenuItemOption : ICloneable
{
    #region Public Properties

    public virtual ICollection<LocalizedText> Description { get; set; } = new List<LocalizedText>();

    [Key]
    public virtual int ID { get; set; }

    public virtual ICollection<LocalizedText> Name { get; set; } = new List<LocalizedText>();

    public virtual MenuItemOptionType OptionType { get; set; }

    public bool ToBeRemoved { get; set; } = false;

    public virtual ICollection<MenuItemOptionValue> Values { get; set; } = new List<MenuItemOptionValue>();

    #endregion Public Properties

    #region Public Methods

    public object Clone() => new MenuItemOption()
    {
        Name = Name.Select(n => (LocalizedText)n.Clone()).ToList(),
        Description = Description.Select(n => (LocalizedText)n.Clone()).ToList(),
        OptionType = OptionType,
        Values = Values.Select(v => (MenuItemOptionValue)v.Clone()).ToList()
    };

    #endregion Public Methods
}