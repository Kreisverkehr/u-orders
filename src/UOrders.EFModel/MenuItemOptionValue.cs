using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UOrders.EFModel;

public class MenuItemOptionValue : ICloneable
{
    #region Public Properties

    [Key]
    public virtual int ID { get; set; }

    public virtual MenuItemOption MenuItemOption { get; set; } = new();

    public virtual ICollection<LocalizedText> Name { get; set; } = new List<LocalizedText>();

    [ForeignKey("MenuItemOptionValuePrice")]
    public virtual ICollection<Price> PriceChangeToBase { get; set; } = new List<Price>();

    public bool ToBeRemoved { get; set; } = false;

    #endregion Public Properties

    #region Public Methods

    public object Clone() => new MenuItemOptionValue()
    {
        Name = Name.Select(n => (LocalizedText)n.Clone()).ToList(),
        PriceChangeToBase = PriceChangeToBase.Select(n => (Price)n.Clone()).ToList(),
    };

    #endregion Public Methods
}