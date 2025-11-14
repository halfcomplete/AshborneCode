using System.Text.RegularExpressions;
using AshborneGame._Core.Globals.Constants;

namespace AshborneGame._Core.Globals.Services
{
    public static class OutputFormatter
    {
        private static readonly Regex QuotedTextRegex = new("\"[^\"]*\"");

        /// <summary>
        /// Replaces quoted text (including quotes) in the input text with appropriate colour tags.
        /// </summary>
        public static string ColourQuotedText(string input, string hexColour)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Replace every quoted section in the input
            return QuotedTextRegex.Replace(input, m =>
            {
                var quoted = m.Value;
                return $"<c={hexColour}>{quoted}</c={hexColour}>";
            });
        }
    }
}
