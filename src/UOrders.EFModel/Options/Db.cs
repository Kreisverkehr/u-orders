namespace UOrders.EFModel.Options;

public sealed class Db
{
    #region Public Fields

    public const string SECTION_NAME = "Db";

    #endregion Public Fields

    #region Public Properties

    public string DbName { get; set; } = string.Empty;

    public string Host { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public int Port { get; set; }

    public string Provider { get; set; } = string.Empty;

    public string User { get; set; } = string.Empty;

    #endregion Public Properties
}