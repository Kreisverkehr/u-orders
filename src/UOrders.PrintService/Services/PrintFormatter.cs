using ESCPOS_NET.Emitters;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;
using UOrders.DTOModel.V1;
using UOrders.PrintService.Extensions;
using UOrders.PrintService.Options;
using UOrders.Shared.Services;

namespace UOrders.PrintService.Services;

internal sealed class PrintFormatter : IPrintFormatter
{
    #region Private Fields

    private readonly ICommandEmitter _emitter;
    private readonly Encoding _encoding;
    private readonly ILogger<PrintFormatter> _logger;
    private readonly IOptions<PrinterSettings> _options;
    private readonly ILinkGenerator _linkGenerator;

    #endregion Private Fields

    #region Public Constructors

    public PrintFormatter(ILogger<PrintFormatter> logger, ICommandEmitter emitter, IOptions<PrinterSettings> options, ILinkGenerator linkGenerator)
    {
        _logger = logger;
        _emitter = emitter;
        _options = options;
        _linkGenerator = linkGenerator;
        _encoding = Encoding.GetEncoding(_options.Value.CodePage);
    }

    #endregion Public Constructors

    #region Public Methods

    public byte[][] FormatOrder(OrderDTO order)
    {
        _logger.LogInformation("Start printing {ID} with CP {CodePage} ({EncodingName})", order.ID, _encoding.CodePage, _encoding.EncodingName);
        var oldCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo(_options.Value.Culture);
        _logger.LogInformation("Culture set to {EnglishName}", CultureInfo.CurrentCulture.EnglishName);

        var printData = new List<byte[]>();

        PrintHeader(printData);

        printData.Add(_emitter.LeftAlign());
        printData.Add(_emitter.FeedLines(1));
        printData.Add(_emitter.PrintLine($"No: {order.ID,-13:0}Dat: {order.OrderedOnLocal:dd.MM.yyyy}", _encoding));
        printData.Add(_emitter.PrintLine($"{order.OrderedOnLocal,27:HH:mm}", _encoding));
        printData.Add(_emitter.PrintLine($"Name: {order.CustomerName/*.Substring(0, 26)*/}", _encoding));
        if (!String.IsNullOrWhiteSpace(order.CustomerPhone))
            printData.Add(_emitter.PrintLine($"Tel: {order.CustomerPhone/*.Substring(0, 27)*/}", _encoding));
        if (!String.IsNullOrWhiteSpace(order.CustomerEmail))
            printData.Add(_emitter.PrintLine($"Mail: {order.CustomerEmail/*.Substring(0, 26)*/}", _encoding));
        printData.Add(_emitter.FeedLines(1));

        foreach (var cat in order.OrderedItems.GroupBy(i => i.MenuItemCategoryTitle).OrderBy(g => g.Key))
        {
            printData.Add(_emitter.SetStyles(PrintStyle.Underline));
            printData.Add(_emitter.PrintLine(cat.Key));
            printData.Add(_emitter.SetStyles(PrintStyle.None));
            foreach (var item in cat)
                PrintItem(printData, item);
        }
        printData.Add(_emitter.PrintLine(new string(Enumerable.Repeat('-', 32).ToArray())));
        printData.Add(_emitter.PrintLine($"Total: {order.TotalPrice,25:c}", _encoding));
        PrintReviewQrCode(printData, order.ID, order.ReviewToken);
        printData.Add(_emitter.FeedLines(3));
        printData.Add(_emitter.FullCut());

        CultureInfo.CurrentCulture = oldCulture;
        _logger.LogInformation("Culture set back to {EnglishName}", CultureInfo.CurrentCulture.EnglishName);
        _logger.LogInformation("Finished printing {ID}", order.ID);

        return printData.ToArray();
    }

    #endregion Public Methods

    #region Private Methods

    private void PrintHeader(IList<byte[]> data)
    {
        data.Add(_emitter.CenterAlign());
        if (_options.Value.UseLogo && _options.Value.LogoPath is not null)
        {
            data.Add(_emitter.PrintImage(File.ReadAllBytes(_options.Value.LogoPath), false, true));
        }
        else
        {
            data.Add(_emitter.SetStyles(PrintStyle.DoubleHeight | PrintStyle.DoubleWidth));
            foreach (var line in _options.Value.HeaderText?.WrapText(16).Split(Environment.NewLine) ?? new string[] { })
                data.Add(_emitter.PrintLine(line, _encoding));
        }
        data.Add(_emitter.SetStyles(PrintStyle.None));
    }

    private void PrintItem(IList<byte[]> data, OrderItemDTO item)
    {
        data.Add(_emitter.PrintLine($" {item.Count,2}x {item.MenuItemTitle,-19} {item.TotalPrice,7:c}", _encoding));

        foreach (var opt in item.CheckedOptions.GroupBy(o => o.OptionValueMenuItemOptionName))
            PrintOption(data, opt);

        if (!string.IsNullOrWhiteSpace(item.Remarks))
        {
            data.Add(_emitter.SetStyles(PrintStyle.FontB));
            data.Add(_emitter.PrintLine($"      \"{item.Remarks}\"", _encoding));
            data.Add(_emitter.SetStyles(PrintStyle.None));
        }
    }

    private void PrintOption(IList<byte[]> data, IGrouping<string, OrderItemOptionDTO> opt)
    {
        data.Add(_emitter.SetStyles(PrintStyle.FontB | PrintStyle.Bold));
        data.Add(_emitter.Print($"      {opt.Key}: ", _encoding));
        data.Add(_emitter.SetStyles(PrintStyle.FontB));
        bool isFirstLine = true;
        foreach (var optValLine in string.Join(", ", opt.Select(o => o.OptionValueName)).WrapText(34 - opt.Key.Length).Split(Environment.NewLine))
        {
            var paddedLine = optValLine;
            if (!isFirstLine)
                paddedLine = new string(Enumerable.Repeat(' ', 8 + opt.Key.Length).ToArray()) + optValLine;
            data.Add(_emitter.PrintLine(paddedLine, _encoding));
            isFirstLine = false;
        }
        data.Add(_emitter.SetStyles(PrintStyle.None));
    }

    private void PrintReviewQrCode(IList<byte[]> data, int orderId, Guid reviewToken)
    {
        data.Add(_emitter.CenterAlign());
        data.Add(_emitter.PrintQRCode(_linkGenerator.GetCreateReviewLink(orderId, reviewToken).ToString(), size: Size2DCode.LARGE));
        data.Add(_emitter.FeedLines(1));
        data.Add(_emitter.PrintLine("Scan me!", _encoding));
        data.Add(_emitter.LeftAlign());
    }

    #endregion Private Methods
}