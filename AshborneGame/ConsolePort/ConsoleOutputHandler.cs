using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

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

#if DEBUG
            int i = 0;
            foreach (var letter in message)
            {
                Console.Write(letter);

                bool isEnd = message.Count() == i + 1 || message[i + 1].Equals(' ') || (i + 2 == message.Length && message.Substring(i + 3).Equals("\n")); // Check if this is the end of the message or sentence
                Thread.Sleep(CharacterOutputDelayCalculator.CalculateDebugDelay(letter, ms, isEnd));

                i += 1;
            }
#else
            int i = 0;
            foreach (var letter in message)
            {
                Console.Write(letter);

                bool isEnd = message.Length == i + 1 || message[i + 1].Equals(' ') || (i + 2 == message.Length && message.Substring(i + 1).Equals("\n")); // Check if this is the end of the message or sentence
                Thread.Sleep(CharacterOutputDelayCalculator.CalculateDelay(letter, ms, isEnd));

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
            if (message.EndsWith(OutputConstants.DialoguePauseMarker))
            {
                var msStr = message.Substring(0, message.IndexOf(OutputConstants.DialoguePauseMarker));
                if (int.TryParse(msStr, out int t))
                {
                    Thread.Sleep(10);
                }
                else
                {
                    Thread.Sleep(OutputConstants.DefaultPauseDuration);
                }
                return;
            }
#if DEBUG
            Write(message, ms);

            int modification = GameContext.Random.Next(OutputConstants.RandomPauseMin, OutputConstants.RandomPauseMax + 1); // Randomly modify the delay
            Console.WriteLine();
            Thread.Sleep((ms + modification) * OutputConstants.NewLinePauseMultiplier);
#else
            Write(message, ms);

            int modification = GameContext.Random.Next(OutputConstants.RandomPauseMin, OutputConstants.RandomPauseMax + 1); // Randomly modify the delay
            Console.WriteLine();
            Thread.Sleep((ms + modification) * OutputConstants.NewLinePauseMultiplier);
#endif
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
