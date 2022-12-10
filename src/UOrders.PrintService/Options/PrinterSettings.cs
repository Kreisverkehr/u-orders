using System.Globalization;

namespace UOrders.PrintService.Options;

public class PrinterSettings
{
    #region Public Fields

    public const string Printer = "Printer";

    #endregion Public Fields

    #region Public Properties

    public int CodePage { get; set; }

    public string? FilePath { get; set; }

    public string? FooterText { get; set; }

    public string? HeaderText { get; set; }

    public string? LogoPath { get; set; }

    public string? NetworkAddress { get; set; }

    public int? NetworkPort { get; set; }

    public string Culture { get; set; } = CultureInfo.CurrentCulture.Name;

    public int? SerialBaudRate { get; set; }

    public string? SerialComPort { get; set; }

    public PrinterType Type { get; set; }

    public bool UseLogo { get; set; } = false;

    #endregion Public Properties
}