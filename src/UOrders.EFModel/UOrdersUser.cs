using Microsoft.AspNetCore.Identity;

namespace UOrders.EFModel;

public class UOrdersUser : IdentityUser
{
    #region Public Properties

    public virtual bool ConfirmationEmailSent { get; set; }

    public virtual string? Name { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    #endregion Public Properties
}