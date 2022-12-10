using UOrders.EFModel;

namespace UOrders.Api.Extensions;

public static class LocalizedTextCollectionExtensions
{
    public static void AddDefaultLanguageIfNeeded(this ICollection<LocalizedText> texts)
    {
        if (texts.FirstOrDefault(t => t.Lang == "") == default)
        {
            texts.Add(new LocalizedText()
            {
                Lang = "",
                Text = texts.Where(t => t.Lang.Length >=2 && t.Lang[..2] == "en").FirstOrDefault()?.Text ?? ""
            });
        }
    }
}
