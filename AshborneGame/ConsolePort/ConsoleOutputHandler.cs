using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using System.Text;

namespace AshborneGame.ConsolePort
{
    public class ConsoleOutputHandler : IOutputHandler
    {
        public void Write(string message)
        {
            Console.Write(message);
        }

        public void Write(string message, int ms)
        {
            // Process inline slow markers in the message
            var (processedMessage, characterSpeeds) = ProcessInlineSlowMarkersWithSpeeds(message, ms);

#if DEBUG
            int i = 0;
            foreach (var letter in processedMessage)
            {
                Console.Write(letter);

                bool isEnd = processedMessage.Length == i + 1 || (i + 1 < processedMessage.Length && processedMessage[i + 1].Equals(' ')) || (i + 2 == processedMessage.Length && processedMessage.Substring(i + 1).Equals("\n")); // Check if this is the end of the message or sentence
                
                // Get the speed for this character (if available)
                int currentSpeed = ms;
                if (characterSpeeds != null && characterSpeeds.Count > i)
                {
                    currentSpeed = characterSpeeds[i];
                }

                // Calculate delay with special handling for non-dialogue output
                int delay;
                
                // First, calculate the base delay using the current speed
                int baseDelay = CharacterOutputDelayCalculator.CalculateDebugDelay(letter, currentSpeed, isEnd);
                
                // Determine if this is dialogue output (when the original ms is the default type speed)
                bool isDialogueOutput = (ms == OutputConstants.DefaultTypeSpeed);
                
                // Then apply non-dialogue multiplier if needed
                if (!isDialogueOutput)
                {
                    // Non-dialogue output - apply 1.2x multiplier only to normal characters
                    // Check if this is a punctuation character that should keep normal speed
                    bool isPunctuation = letter == '.' || letter == '—' || letter == ',' || letter == '"' || letter == ':' || letter == ']' || letter == ')';
                    
                    if (isPunctuation)
                    {
                        delay = baseDelay; // Keep normal speed for punctuation
                    }
                    else
                    {
                        delay = (int)(baseDelay * OutputConstants.NonDialogueOutputSpeedMultiplier); // Apply 1.2x for normal characters
                    }
                }
                else
                {
                    // Dialogue output - use the base delay as calculated
                    delay = baseDelay;
                }

                Thread.Sleep(delay);
                i += 1;
            }
#else
            int i = 0;
            foreach (var letter in processedMessage)
            {
                Console.Write(letter);

                bool isEnd = processedMessage.Length == i + 1 || (i + 1 < processedMessage.Length && processedMessage[i + 1].Equals(' ')) || (i + 2 == processedMessage.Length && processedMessage.Substring(i + 1).Equals("\n")); // Check if this is the end of the message or sentence
                
                // Get the speed for this character (if available)
                int currentSpeed = ms;
                if (characterSpeeds != null && characterSpeeds.Count > i)
                {
                    currentSpeed = characterSpeeds[i];
                }

                // Calculate delay with special handling for non-dialogue output
                int delay;

                // First, calculate the base delay using the current speed
                int baseDelay = CharacterOutputDelayCalculator.CalculateDelayAsync(letter, currentSpeed, isEnd).GetAwaiter().GetResult();

                // Determine if this is dialogue output (when the original ms is the default type speed)
                bool isDialogueOutput = (ms == OutputConstants.DefaultTypeSpeed);
                
                // Then apply non-dialogue multiplier if needed
                if (!isDialogueOutput)
                {
                    // Non-dialogue output - apply 1.2x multiplier only to normal characters
                    // Check if this is a punctuation character that should keep normal speed
                    bool isPunctuation = letter == '.' || letter == '—' || letter == ',' || letter == '"' || letter == ':' || letter == ']' || letter == ')';
                    
                    if (isPunctuation)
                    {
                        delay = baseDelay; // Keep normal speed for punctuation
                    }
                    else
                    {
                        delay = (int)(baseDelay * OutputConstants.NonDialogueOutputSpeedMultiplier);
                    }
                }
                else
                {
                    // Dialogue output - use the base delay as calculated
                    delay = baseDelay;
                }

                Thread.Sleep(delay);
                i += 1;
            }
#endif
        }

        public void WriteLine(string message)
        {
            // Write a line with default delay
            WriteLine(message, 30);
        }

        public void WriteLine(string message, int ms)
        {
            // Handle special pause marker: ms__PAUSE__
            if (message.EndsWith("__PAUSE__"))
            {
                var msStr = message.Substring(0, message.IndexOf("__PAUSE__"));
                if (int.TryParse(msStr, out int t))
                {
                    Thread.Sleep(t);
                }
                else
                {
                    Thread.Sleep(OutputConstants.DefaultPauseDuration);
                }
                return;
            }

            // Handle new line marker: __NL__
            if (message.Contains("__NL__"))
            {
                // Replace __NL__ with actual newline
                message = message.Replace("__NL__", "\n");
            }

            // Handle typewriter markers: ms__TS__message__TE__
            if (message.Contains(OutputConstants.TypewriterStartMarker) && message.Contains(OutputConstants.TypewriterEndMarker))
            {
                int msEnd = message.IndexOf(OutputConstants.TypewriterStartMarker);
                if (msEnd > 0)
                {
                    string msStr = message.Substring(0, msEnd);
                    if (int.TryParse(msStr, out int typewriterMs))
                    {
                        ms = typewriterMs;
                    }
                }
                
                int startIndex = message.IndexOf(OutputConstants.TypewriterStartMarker) + OutputConstants.TypewriterStartMarker.Length;
                int endIndex = message.IndexOf(OutputConstants.TypewriterEndMarker);
                string typewriterMessage = message.Substring(startIndex, endIndex - startIndex);
                
                Write(typewriterMessage, ms);
                return;
            }

#if DEBUG
            Write(message, ms);

            int modification = GameContext.Random.Next(OutputConstants.RandomPauseMin, OutputConstants.RandomPauseMax + 1); // Randomly modify the delay
            Console.WriteLine();
            int adjustedMs = ms == OutputConstants.DefaultTypeSpeed ? ms : (int)(ms * OutputConstants.NonDialogueOutputSpeedMultiplier); // Only apply multiplier if not dialogue
            Thread.Sleep((adjustedMs + modification) * OutputConstants.NewLinePauseMultiplier);
#else
            Write(message, ms);

            int modification = GameContext.Random.Next(OutputConstants.RandomPauseMin, OutputConstants.RandomPauseMax + 1); // Randomly modify the delay
            Console.WriteLine();
            int adjustedMs = ms == OutputConstants.DefaultTypeSpeed ? ms : (int)(ms * OutputConstants.NonDialogueOutputSpeedMultiplier); // Only apply multiplier if not dialogue
            Thread.Sleep((adjustedMs + modification) * OutputConstants.NewLinePauseMultiplier);
#endif
        }

        private (string processedMessage, List<int> characterSpeeds) ProcessInlineSlowMarkersWithSpeeds(string message, int defaultMs)
        {
            var processedMessage = new StringBuilder();
            var characterSpeeds = new List<int>();
            int currentIndex = 0;

            while (currentIndex < message.Length)
            {
                // 1) find the next "<~>"
                int inlineStartIndex = message.IndexOf(
                    OutputConstants.InlineTypewriterStartMarker,
                    currentIndex);
                if (inlineStartIndex == -1)
                {
                    // no more markers → dump the rest
                    var tail = message.Substring(currentIndex);
                    processedMessage.Append(tail);
                    characterSpeeds.AddRange(
                        Enumerable.Repeat(defaultMs, tail.Length));
                    break;
                }

                // 2) scan backwards from inlineStartIndex to grab any digits
                int speedStartIndex = inlineStartIndex;
                while (speedStartIndex > currentIndex
                       && char.IsDigit(message[speedStartIndex - 1]))
                    speedStartIndex--;

                // 3) everything between currentIndex and speedStartIndex is "beforeText"
                var beforeText = message.Substring(
                    currentIndex,
                    speedStartIndex - currentIndex);
                processedMessage.Append(beforeText);
                characterSpeeds.AddRange(
                    Enumerable.Repeat(defaultMs, beforeText.Length));

                // 4) parse the speed override (if any)
                int inlineSpeed = defaultMs;
                if (speedStartIndex < inlineStartIndex)
                {
                    var speedStr = message.Substring(
                        speedStartIndex,
                        inlineStartIndex - speedStartIndex);
                    if (int.TryParse(speedStr, out var parsed))
                        inlineSpeed = parsed;
                }

                // 5) find the matching end‐marker
                int inlineEndIndex = message.IndexOf(
                    OutputConstants.InlineTypewriterEndMarker,
                    inlineStartIndex);
                if (inlineEndIndex == -1)
                {
                    // unmatched → treat rest as normal text
                    var tail = message.Substring(inlineStartIndex);
                    processedMessage.Append(tail);
                    characterSpeeds.AddRange(
                        Enumerable.Repeat(defaultMs, tail.Length));
                    break;
                }

                // 6) extract the text *inside* the markers
                int inlineTextStart = inlineStartIndex
                                      + OutputConstants
                                         .InlineTypewriterStartMarker.Length;
                var inlineText = message.Substring(
                    inlineTextStart,
                    inlineEndIndex - inlineTextStart);

                processedMessage.Append(inlineText);
                characterSpeeds.AddRange(
                    Enumerable.Repeat(inlineSpeed, inlineText.Length));

                // 7) advance past the end‐marker
                currentIndex = inlineEndIndex
                               + OutputConstants
                                 .InlineTypewriterEndMarker.Length;
            }

            return (processedMessage.ToString(), characterSpeeds);
        }

        public void DisplayDebugMessage(string message, ConsoleMessageTypes type)
        {
            switch (type)
            {
                case ConsoleMessageTypes.INFO:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case ConsoleMessageTypes.WARNING:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case ConsoleMessageTypes.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }

#if DEBUG
            Console.WriteLine($"[{type}]: {message}");
#endif

            Console.ResetColor();
        }

        public void DisplayFailMessage(string message)
        {
            Console.WriteLine($"[FAIL] {message}");
        }
    }
}
