namespace UOrders.Api.Options;

public class JwtOptions
{
    #region Public Fields

    public const string SECTION_NAME = "JWT";

    #endregion Public Fields

    #region Public Properties

    public string Secret { get; set; } = string.Empty;

    public string ValidAudience { get; set; } = string.Empty;

    public string ValidIssuer { get; set; } = string.Empty;

    #endregion Public Properties
}