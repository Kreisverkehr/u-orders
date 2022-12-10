namespace UOrders.Api.Options;

public sealed class PrinterOptions
{
    #region Public Fields

    public static string SECTION_NAME = "Printer";

    #endregion Public Fields

    #region Public Properties

    public string Host { get; set; } = string.Empty;

    public string Lang { get; set; } = string.Empty;

    public int Port { get; set; }

    public string Secret { get; set; } = string.Empty;

    #endregion Public Properties
}