using System.ComponentModel.DataAnnotations;

namespace UOrders.EFModel;

public class OrderItem
{
    #region Public Properties

    public virtual ICollection<OrderItemCheckedOption> CheckedOptions { get; set; } = new List<OrderItemCheckedOption>();

    public virtual int Count { get; set; }

    [Key]
    public virtual int ID { get; set; }

    public virtual MenuItem MenuItem { get; set; } = new();

    public virtual Order Order { get; set; } = new();

    public virtual string? Remarks { get; set; }

    #endregion Public Properties
}