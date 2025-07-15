using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;

namespace AshborneGame.ConsolePort
{
    public class ConsoleOutputHandler : IOutputHandler
    {
        public void Write(string message)
        {
            Console.Write(message);
        }
        public void WriteLine(string message)
        {
            // Write a line with default delay
            Console.WriteLine(message, 30);
        }

        public void WriteLine(string message, int ms)
        {
            
#if DEBUG
            int i = 0;
            foreach (var letter in message)
            {
                int modification = GameContext.Random.Next(-5, 6); // Randomly modify the delay by -5 to +5 milliseconds
                Console.Write(letter);
                if (letter.Equals('.') && (message.Count() == i + 1 || message[i + 1].Equals(' '))) // If we reach a full stop and it's the last in the message / sentence.
                {
                    Thread.Sleep((ms + modification) * 7);
                }
                else if ((letter.Equals('"') || letter.Equals(':')) && (message.Count() == i + 1 || message[i + 1].Equals(' ') || message[i + 1].Equals('\n'))) // If we reach a quotation mark or colon and it's the end of the message / sentence
                {
                    Thread.Sleep((ms + modification) * 6);
                }
                else if (letter.Equals(']') && (message.Count() == i + 1 || message[i + 1].Equals(' '))) // If we reach a closing square bracket and it's the end of the message / sentence
                {
                    Thread.Sleep((ms + modification) * 7);
                }
                else
                {
                    Thread.Sleep((ms + modification) / 2);
                }

                if (message.Count() == i + 1)
                {
                    Console.WriteLine();
                    Thread.Sleep((ms + modification) * 2);
                }

                i += 1;
            }
#else
            int i = 0;
            foreach (var letter in message)
            {
                int modification = GameContext.Random.Next(-5, 6); // Randomly modify the delay by -5 to +5 milliseconds
                Console.Write(letter);
                if (letter.Equals('.') && (message.Count() == i + 1 || message[i + 1].Equals(' '))) // If we reach a full stop and it's the last in the message / sentence.
                {
                    Thread.Sleep((ms + modification) * 14);
                }
                else if ((letter.Equals('"') || letter.Equals(':')) && (message.Count() == i + 1 || message[i + 1].Equals(' ') || message[i + 1].Equals('\n'))) // If we reach a quotation mark or colon and it's the end of the message / sentence
                {
                    Thread.Sleep((ms + modification) * 12);
                }
                else if (letter.Equals(']') && (message.Count() == i + 1 || message[i + 1].Equals(' '))) // If we reach a closing square bracket and it's the end of the message / sentence
                {
                    Thread.Sleep((ms + modification) * 14);
                }
                else
                {
                    Thread.Sleep((ms + modification));
                }

                if (message.Count() == i + 1)
                {
                    Console.WriteLine();
                    Thread.Sleep((ms + modification) * 4);
                }

                i += 1;
            }
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
            //Console.WriteLine($"[{type}]: {message}");
#endif

            Console.ResetColor();
        }

        public void DisplayFailMessage(string message)
        {
            Console.WriteLine($"[FAIL] {message}");
        }
    }
}
