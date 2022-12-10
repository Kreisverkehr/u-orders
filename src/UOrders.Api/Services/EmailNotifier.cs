using System.Net.Mail;

namespace UOrders.Api.Services;

internal sealed class EmailNotifier : INotifier
{
    #region Private Fields

    private readonly ILogger<EmailNotifier> _logger;
    private readonly IOrdersRepository _ordersRepository;
    private readonly SmtpClient _smtpClient;
    private readonly IEmailBuilder _emailBuilder;

    #endregion Private Fields

    #region Public Constructors

    public EmailNotifier(IOrdersRepository ordersRepository, SmtpClient smtpClient, IEmailBuilder emailBuilder, ILogger<EmailNotifier> logger)
    {
        _ordersRepository = ordersRepository;
        _smtpClient = smtpClient;
        _emailBuilder = emailBuilder;
        _logger = logger;
    }

    #endregion Public Constructors

    #region Public Methods

    public bool CanBeNotified(int orderId)
    {
        var order = _ordersRepository.GetOrder(orderId);
        return order?.CustomerEmail != null;
    }

    public void Notify(int orderId)
    {
        _logger.LogInformation("Notifying customer of {orderId}", orderId);
        var order = _ordersRepository.GetOrder(orderId) ?? new();
        var mail = _emailBuilder.FromOrder(order);
        try
        {
            _smtpClient.Send(mail);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send mail.");
        }
    }

    #endregion Public Methods
}