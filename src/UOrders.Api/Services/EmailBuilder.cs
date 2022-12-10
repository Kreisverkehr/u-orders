using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Web;
using System.Xml.Linq;
using UOrders.Api.Options;
using UOrders.DTOModel.V1;
using UOrders.EFModel;
using UOrders.Shared.Options;

namespace UOrders.Api.Services;

internal sealed class EmailBuilder : IEmailBuilder
{
    #region Private Fields

    private readonly IOptions<MailOptions> _mailOptions;

    #endregion Private Fields

    #region Public Constructors

    public EmailBuilder(IOptions<MailOptions> mailOptions)
    {
        _mailOptions = mailOptions;
    }

    #endregion Public Constructors

    #region Public Methods

    public MailMessage FromOrder(OrderDTO order)
    {
        if (order.CustomerEmail == null)
            throw new InvalidOperationException("Cannot build E-Mail without CustomerEmail.");

        var mail = new MailMessage();
        mail.From = new(_mailOptions.Value.SenderEmail);
        mail.To.Add(new MailAddress(order.CustomerEmail, order.CustomerName));
        mail.Subject = $"Your order #{order.ID:000000}";
        mail.IsBodyHtml = true;

        XNamespace h = "http://www.w3.org/1999/xhtml";
        mail.Body = new XElement(h + "html",
            new XElement(h + "head",
                new XElement(h + "title", $"Your order #{order.ID:000000}")
            ),
            new XElement(h + "body",
                new XElement(h + "p", $"Hi {order.CustomerName},"),
                new XElement(h + "p", $"We've received your order. Please find the details below."),
                new XElement(h + "table",
                    new XElement(h + "thead",
                        new XElement(h + "tr",
                            new XElement(h + "th", "Title"),
                            new XElement(h + "th", "Quantity"),
                            new XElement(h + "th", "Price")
                        )
                    ),
                    new XElement(h + "tbody",
                        from orderPos in order.OrderedItems
                        select new XElement(h + "tr",
                            new XElement(h + "td", orderPos.MenuItemTitle),
                            new XElement(h + "td", orderPos.Count),
                            new XElement(h + "td", orderPos.TotalPrice)
                        )
                    )
                ),
                new XElement(h + "p", $"Total: {order.TotalPrice}")
            )
        ).ToString();

        return mail;
    }

    #endregion Public Methods
}

public interface IEmailBuilder
{
    #region Public Methods

    MailMessage FromOrder(OrderDTO order);

    #endregion Public Methods
}