using System.Text;

namespace UOrders.PrintService.Extensions
{
    public static class StringExtensions
    {
        #region Public Methods

        public static string WrapText(this string text, int maxCharsPerLine)
        {
            string[] words = text.Split(' ');
            List<string> resultLines = new List<string>();
            StringBuilder actualLine = new StringBuilder();

            for (int i = 0; i < words.Length; i++)
            {
                if (actualLine.Length + words[i].Length > maxCharsPerLine)
                {
                    resultLines.Add(actualLine.ToString());
                    actualLine.Clear();
                }

                actualLine.Append(words[i]);
                actualLine.Append(' ');
            }

            if (actualLine.Length > 0)
                resultLines.Add(actualLine.ToString());

            return string.Join(Environment.NewLine, resultLines);
        }

        public static string UnifyLineBreaks(this string text) =>
            text.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);

        #endregion Public Methods
    }
}