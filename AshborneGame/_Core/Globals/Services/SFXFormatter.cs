using AshborneGame._Core.Globals.Constants;
using System.Text;
using System.Text.RegularExpressions;

namespace AshborneGame._Core.Globals.Services
{
    /// <summary>
    /// Handles conversion of text special effects (SFX) markers to HTML spans with CSS classes.
    /// SFX markers have the format: &lt;sfx="effect-name"&gt;text&lt;/sfx="effect-name"&gt;
    /// </summary>
    public static class SFXFormatter
    {
        /// <summary>
        /// Replaces SFX markers in the input text with appropriate HTML spans containing CSS classes for animations.
        /// Example: &lt;sfx="shake"&gt;shaking text&lt;/sfx="shake"&gt; becomes &lt;span class="sfx-shake"&gt;shaking text&lt;/span&gt;
        /// </summary>
        /// <param name="input">The input text containing SFX markers</param>
        /// <returns>The text with SFX markers converted to HTML spans</returns>
        public static string ConvertSFXToSpans(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return OutputConstants.SFXTagRegex.Replace(input, m =>
            {
                var effectName = m.Groups[1].Value;
                var content = m.Groups[2].Value;

                var sb = new StringBuilder();
                sb.Append($"<span class=\"{effectName}\">");

                // Parse content and wrap only actual text characters, not HTML tags
                int charIndex = 0;
                int i = 0;
                while (i < content.Length)
                {
                    // Check if we're at the start of an HTML tag
                    if (content[i] == '<')
                    {
                        // Find the end of the tag
                        int tagEnd = content.IndexOf('>', i);
                        if (tagEnd != -1)
                        {
                            // Append the entire HTML tag as-is
                            sb.Append(content.Substring(i, tagEnd - i + 1));
                            i = tagEnd + 1;
                            continue;
                        }
                    }

                    // Regular character - wrap it
                    char c = content[i];
                    if (c == ' ')
                    {
                        sb.Append(" ");
                    }
                    else
                    {
                        sb.Append($"<span class=\"sfx-char\" style=\"--i:{charIndex}\">{c}</span>");
                        charIndex++;
                    }
                    i++;
                }

                sb.Append("</span>");
                return sb.ToString();
            });
        }

    }
}
