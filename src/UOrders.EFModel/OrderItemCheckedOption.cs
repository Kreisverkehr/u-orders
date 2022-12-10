using System.ComponentModel.DataAnnotations;

namespace UOrders.EFModel;

public class OrderItemCheckedOption
{
    #region Public Properties

    [Key]
    public virtual int ID { get; set; }

    public virtual MenuItemOptionValue OptionValue { get; set; } = new();

    public virtual OrderItem OrderItem { get; set; } = new();

    #endregion Public Properties
}