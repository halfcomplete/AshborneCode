using System.Text.RegularExpressions;

namespace AshborneGame._Core.Globals.Constants
{
    public static class OutputConstants
    {
        /// <summary>
        /// The marker that indicates the start of an inline italic section within typewriter text.
        /// </summary>
        public const string InlineItalicStartMarker = "<i>";
        /// <summary>
        /// The marker that indicates the end of an inline italic section within typewriter text.
        /// </summary>
        public const string InlineItalicEndMarker = "</i>";

        public static readonly Regex SpeakerTagRegex = new(
            "__S:([A-Za-z]+)__", RegexOptions.Compiled);

        // replace all matches of speech with an enclosing colour marker using isNextLineColoured
        public static readonly Regex SpeechRegex = new(
            "\"[^\"]*\"", RegexOptions.Compiled);


        // <c=AABBCC>...</c=AABBCC>
        public static readonly Regex ColourTagRegexStrict = new(
        @"<c=([0-9A-Fa-f]{6})>(.*?)</c=\1>",
        RegexOptions.Singleline | RegexOptions.Compiled);

        // <c=AABBCC>...<c=AABBCC> (missing slash)
        // We accept a closing tag that mistakenly omits '/', and still convert it.
        public static readonly Regex ColourTagRegexLenient = new(
        @"<c=([0-9A-Fa-f]{6})>(.*?)<c=\1>",
        RegexOptions.Singleline | RegexOptions.Compiled);

        // <sfx=effect-name>...</sfx=effect-name> - Text special effects marker
        public static readonly Regex SFXTagRegex = new(
        @"<sfx=([^>]+)>(.*?)</sfx=\1>",
        RegexOptions.Singleline | RegexOptions.Compiled);

        public static readonly Regex InkFunctionRegex = new(
            @"""ev"",([^{]+),\{""x\(\)"":""([^""]*)"",""exArgs"":(\d)\}",
            RegexOptions.Compiled);

        /// <summary>
        /// Default type speed for Release builds in milliseconds per character.
        /// </summary>
        public const int DefaultTypeSpeed = 22;

        /// <summary>
        /// Multiplier for typewriter speed (user adjustable, e.g. 1.0 = normal, 0.5 = half speed, 3.0 = triple speed).
        /// </summary>
        private static double _typeSpeedMultiplier = 1.0;
        public static double TypeSpeedMultiplier
        {
            get => _typeSpeedMultiplier;
            set => _typeSpeedMultiplier = Math.Clamp(value, 0.5, 3.0);
        }

        /// <summary>
        /// Whether the game output/timers are currently paused (e.g. when settings modal is open).
        /// </summary>
        public static bool Paused { get; set; } = false;

        /// <summary>
        /// Helper to set type speed multiplier from UI slider.
        /// </summary>
        public static void SetTypeSpeedMultiplier(double multiplier)
        {
            TypeSpeedMultiplier = multiplier;
        }
        /// <summary>
        /// Default type speed modifiers for Debug builds as a multiplier for DefaultTypeSpeed.
        /// </summary>
        public const float DefaultDebugTypeSpeedModifier = 0.01f;
      
        /// <summary>
        /// The multiplier for pauses after a full stop.
        /// </summary>
        public const int FullStopPauseMultiplier = 14;

        /// <summary>
        /// The multiplier for pauses after a comma.
        /// </summary>
        public const int CommaPauseMultiplier = 6;

        /// <summary>
        /// The multiplier for pauses after an em dash.
        /// </summary>
        public const int EmDashPauseMultiplier = 7;
      
        /// <summary>
        /// The multiplier for pauses after a double quotation mark.
        /// </summary>
        public const int QuotationPauseMultiplier = 12;

        /// <summary>
        /// The multiplier for pauses after a closing square bracket.
        /// </summary>
        public const int ClosingSquareBracketPauseMultiplier = 14;

        /// <summary>
        /// The multiplier for pauses after a closing bracket.
        /// </summary>
        public const int ClosingParenthesisPauseMultiplier = 13;
        /// <summary>
        /// The multiplier for pauses after a colon.
        /// </summary>
        public const int ColonPauseMultiplier = 9;

        /// <summary>
        /// The multiplier for pauses after a semicolon.
        /// </summary>
        public const int SemicolonPauseMultiplier = 9;
        /// <summary>
        /// The multiplier for pauses after a question mark.
        /// </summary>
        public const int QuestionMarkPauseMultiplier = 11;
        /// <summary>
        /// The multiplier for pauses after an exclamation mark.
        /// </summary>
        public const int ExclamationMarkPauseMultiplier = 12;
        /// <summary>
        /// The multiplier for pauses after a new line.
        /// </summary>
        public const int NewLinePauseMultiplier = 7;
      
        /// <summary>
        /// Inclusive minimum random pause duration in milliseconds.
        /// </summary>
        public const int RandomPauseMin = -5;

        /// <summary>
        /// Inclusive maximum random pause duration in milliseconds.
        /// </summary>
        public const int RandomPauseMax = 5;

        /// <summary>
        /// The marker that indicates this line should be processed as a pause effect. Positioned at the end of the line after the pause duration in milliseconds.
        /// </summary>
        public const string DialoguePauseMarker = "__PAUSE__";

        /// <summary>
        /// The tag that indicates the dialogue speed for the next line.
        /// </summary>
        public const string DialogueSpeedTag = "#slow:";

        /// <summary>
        /// Default pause duration in milliseconds when a pause duration is not specified in a __PAUSE__ marker.
        /// </summary>
        public const int DefaultPauseDuration = 1000;

        /// <summary>
        /// The marker that indicates the start of a message that should be displayed with typewriter effect.
        /// </summary>
        public const string TypewriterStartMarker = "__TS__";
        /// <summary>
        /// The marker that indicates the end of a message that should be displayed with typewriter effect.
        /// </summary>
        public const string TypewriterEndMarker = "__TE__";
        /// <summary>
        /// The marker that indicates the start of an inline slow section within typewriter text.
        /// Format: [speed]__IS__[text]__IE__
        /// </summary>
        public const string InlineTypewriterStartMarker = "<~>";
        /// <summary>
        /// The marker that indicates the end of an inline slow section within typewriter text.
        /// </summary>
        public const string InlineTypewriterEndMarker = @"</~>";
        /// <summary>
        /// The marker that indicates typed player input should be requested at this point in the ink file.
        /// </summary>
        public const string GetPlayerInputMarker = "__GET_PLAYER_INPUT__";

        /// <summary>
        /// The marker that indicates a new line in the output.
        /// </summary>
        public const string NewLineMarker = "__NL__";

        /// <summary>
        /// The marker that indicates the end of the ink file.
        /// </summary>
        public const string DialogueEndMarker = "__END__";

        public const string ForceMaskMarker = "__FORCE_MASK__";

        /// <summary>
        /// The marker that indicates a blur animation effect should occur.
        /// Format: __ANIMATE_BLUR__targetOpacity__durationSecs__fadeBackDurationSecs__waitSecs
        /// targetOpacity: 0.0 to 1.0 (target blur opacity to fade to)
        /// durationSecs: duration in seconds for fading to target opacity
        /// fadeBackDurationSecs: duration in seconds to fade back to full transparency (-1 = no fade back)
        /// waitSecs: duration in seconds to wait between fade-in and fade-out (ignored if fadeBackDurationSecs is -1)
        /// </summary>
        public const string BlurAnimationMarker = "__ANIMATE_BLUR__";

        /// <summary>
        /// Regex to parse blur animation markers with capture groups for (targetOpacity, durationSecs, fadeBackDurationSecs, waitSecs)
        /// </summary>
        public static readonly Regex BlurAnimationRegex = new(
            @"__ANIMATE_BLUR__([0-9]*\.?[0-9]+)__([0-9]*\.?[0-9]+)__(-?[0-9]*\.?[0-9]+)__([0-9]*\.?[0-9]+)",
            RegexOptions.Compiled);

        /// <summary>
        /// Multiplier for non-dialogue output speed (e.g., 1.2x faster).
        /// </summary>
        public const float NonDialogueOutputSpeedMultiplier = 3.5f;

        public static readonly int DefaultNonDialogueOutputSpeed = (int)Math.Round(DefaultTypeSpeed / NonDialogueOutputSpeedMultiplier);

        public const string ShortenedCentre = "centre";
        public const string ShortenedRight = "right";
        public const string ShortenedLeft = "left";
        public const string ShortenedAtTop = "at top";
        public const string ShortenedAtBottom = "at bottom";
        public const string ShortenedAtFront = "at front";
        public const string ShortenedAtBack = "at back";
        public const string ShortenedAtMiddle = "at middle";
        public const string ShortenedOnTop = "on top";
        public const string ShortenedOnBottom = "on bottom";
        public const string ShortenedInFront = "in front";
        public const string ShortenedInBack = "in back";
        public const string ShortenedInMiddle = "in middle";
        public const string ShortenedBehind = "behind";
        public const string ShortenedAbove = "above";
        public const string ShortenedBelow = "below";
    }
}
