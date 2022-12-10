using Blazorise;
using System.Globalization;

namespace UOrders.WebUI.Options;

public class BrandOptions
{
    #region Public Fields

    public const string Brand = "Brand";

    #endregion Public Fields

    #region Private Fields

    private IDictionary<string, IFormatProvider>? _currencies = null;

    #endregion Private Fields

    #region Public Properties

    public string Currency { get; set; } = String.Empty;

    public IFormatProvider CurrencyFormatProvider
    {
        get
        {
            if (_currencies == null)
            {
                _currencies = new Dictionary<string, IFormatProvider>();
                var currencies =
                    from culture in CultureInfo.GetCultures(CultureTypes.AllCultures)
                    where culture != null
                    where !culture.IsNeutralCulture
                    where culture != CultureInfo.InvariantCulture
                    let region = new RegionInfo(culture.Name)
                    select new { region.ISOCurrencySymbol, culture.NumberFormat };
                ;
                foreach (var currency in currencies)
                {
                    if (!_currencies.ContainsKey(currency.ISOCurrencySymbol))
                    {
                        NumberFormatInfo numberFormat = (NumberFormatInfo)Thread.CurrentThread.CurrentCulture.NumberFormat.Clone();
                        numberFormat.CurrencyDecimalDigits = currency.NumberFormat.CurrencyDecimalDigits;
                        numberFormat.CurrencySymbol = currency.NumberFormat.CurrencySymbol;
                        _currencies.Add(currency.ISOCurrencySymbol, numberFormat);
                    }
                }
            }
            return _currencies[Currency];
        }
    }

    public string Description { get; set; } = string.Empty;

    public IconName Icon { get; set; }

    public string Name { get; set; } = string.Empty;

    #endregion Public Properties
}