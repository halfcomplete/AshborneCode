
namespace AshborneGame._Core.Globals.Constants
{
    public static class OutputConstants
    {
        /// <summary>
        /// Default type speed for Release builds in milliseconds per character.
        /// </summary>
        public const int DefaultTypeSpeed = 29;
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
        public const string TypewriterPauseMarker = "__PAUSE__";
        /// <summary>
        /// Default pause duration in milliseconds when a pause duration is not specified in a __PAUSE__ marker.
        /// </summary>
        public const int DefaultPauseDuration = 1000;
        /// <summary>
        /// The marker that indicates the start of a message that should be displayed with typewriter effect.
        /// </summary>
        public const string TypewriterStartMarker = "__TYPEWRITER_START__";
        /// <summary>
        /// The marker that indicates the end of a message that should be displayed with typewriter effect.
        /// </summary>
        public const string TypewriterEndMarker = "__TYPEWRITER_END__";
        /// <summary>
        /// The marker that indicates a player input is required in Ink.
        /// </summary>
        public const string PlayerInputMarker = "__GET_PLAYER_INPUT__";
        /// <summary>
        /// The marker that indicatees the end of a dialogue in Ink.
        /// </summary>
        public const string DialogueEndMarker = "__END__";
    }
}
