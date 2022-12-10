namespace UOrders.Shared.Options;

public sealed class MailOptions
{
    #region Public Fields

    public static string SECTION_NAME = "Mail";

    #endregion Public Fields

    #region Public Properties

    public string Password { get; set; } = string.Empty;

    public string SenderEmail { get; set; } = "noreply@uorders.example.com";

    public string SmtpHost { get; set; } = string.Empty;

    public int SmtpPort { get; set; } = 1025;

    public string User { get; set; } = string.Empty;

    public bool UseSsl { get; set; } = false;

    #endregion Public Properties
}