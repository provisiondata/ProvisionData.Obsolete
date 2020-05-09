namespace ProvisionData.Extensions
{
    using System;
    using System.Linq;

    public static class TextToHtmlExtensions
    {
        //private static readonly Regex Line = new Regex("^(.*)$", RegexOptions.Multiline | RegexOptions.Compiled);

        //public static String ToHtmlParagraphs(this String value)
        //{
        //    if (String.IsNullOrWhiteSpace(value)) return value;

        //    var lines = value.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        //    return String.Join(Environment.NewLine, lines.Select(Wrap));

        //    static String Wrap(String line) => Line.Replace(line, "<p>$1</p>");
        //}

        /// <summary>
        /// Wraps each line in <paramref name="text"/> in HTML &lt;p&gt;&lt;/p&gt; tags.
        /// </summary>
        /// <param name="text">plain, multi-line text</param>
        /// <returns>HTML string</returns>
        /// <example>
        /// "Hello\r\nWorld!"
        /// becomes
        /// "<p>Hello</p>\r\n<p>World!</p>"
        /// </example>
        public static String ToHtmlParagraphs(this String text)
        {
            if (String.IsNullOrWhiteSpace(text)) return text;

            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            return String.Join(Environment.NewLine, lines.Select(l => $"<p>{l}</p>"));
        }
    }
}
