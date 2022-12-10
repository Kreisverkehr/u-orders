namespace UOrders.Api.Extensions;

public static class StringExtensions
{
    public static string ShortenName(this string name)
    {
        var nameParts = name.Split(' ');
        nameParts[^1] = nameParts[^1][0] + ".";
        return string.Join(" ", nameParts);
    }
}
