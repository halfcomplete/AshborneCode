using System.Text.RegularExpressions;
using AshborneGame._Core.Globals.Constants;

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

            // Replace every SFX tag in the input
            return OutputConstants.SFXTagRegex.Replace(input, m =>
            {
                var effectName = m.Groups[1].Value;
                var content = m.Groups[2].Value;
                return $"<span class=\"{effectName}\">{content}</span>";
            });
        }
    }
}
