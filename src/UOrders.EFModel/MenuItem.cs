using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UOrders.EFModel;

public class MenuItem
{
    #region Public Properties

    public virtual MenuCategory Category { get; set; } = new();

    public virtual ICollection<LocalizedText> Description { get; set; } = new List<LocalizedText>();

    [Key]
    public virtual int ID { get; set; }

    public virtual ICollection<MenuItemOption> Options { get; set; } = new List<MenuItemOption>();

    [ForeignKey("MenuItemPrice")]
    public virtual ICollection<Price> Prices { get; set; } = new List<Price>();

    public virtual ICollection<LocalizedText> Title { get; set; } = new List<LocalizedText>();

    public bool ToBeRemoved { get; set; } = false;

    #endregion Public Properties
}