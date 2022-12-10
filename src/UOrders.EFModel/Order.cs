using System.ComponentModel.DataAnnotations;

namespace UOrders.EFModel;

public class Order
{
    #region Public Properties

    public virtual string? CustomerEmail { get; set; }

    public virtual string CustomerName { get; set; } = string.Empty;

    public virtual string? CustomerPhone { get; set; }

    [Key]
    public virtual int ID { get; set; }

    public virtual ICollection<OrderItem> OrderedItems { get; set; } = new List<OrderItem>();

    public virtual DateTimeOffset OrderedOn { get; set; } = DateTimeOffset.Now;

    public virtual string? Remarks { get; set; } = string.Empty;

    public virtual UOrdersUser? OrderedBy { get; set; }

    public virtual Guid? ReviewToken { get; set; } = Guid.NewGuid();

    public virtual ICollection<OrderReview> Reviews { get; set; } = new List<OrderReview>();

    #endregion Public Properties
}
