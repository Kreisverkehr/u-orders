namespace UOrders.WebUI.Model;

public class LocalizedText
{
    #region Public Constructors

    public LocalizedText()
    { }

    public LocalizedText(string lang) : this()
    {
        Lang = lang;
    }

    public LocalizedText(string lang, string text) : this(lang)
    {
        Text = text;
    }

    #endregion Public Constructors

    #region Public Properties

    public string Lang { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    #endregion Public Properties
}