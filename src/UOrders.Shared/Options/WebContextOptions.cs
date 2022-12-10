namespace UOrders.Shared.Options;

public sealed class WebContextOptions
{
    #region Public Fields

    public static string SECTION_NAME = "WebContext";

    #endregion Public Fields

    #region Public Properties

    public string ApiBaseUri { get; set; } = "https://localhost:7074/api/v1/";

    public string WebUiBaseUri { get; set; } = "https://localhost:7188/";

    #endregion Public Properties
}