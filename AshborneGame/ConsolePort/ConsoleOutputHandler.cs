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
#if DEBUG
            Console.WriteLine(message);
#else
            {
                int i = 0;
                foreach (var letter in message)
                {
                    Console.Write(letter);
                    if (letter.Equals('.') && (message.Count() == i + 1 || message[i + 1].Equals(' '))) // If we reach a full stop and it's the last in the message / sentence.
                    {
                        Thread.Sleep(130);
                    }
                    else if ((letter.Equals('"') || letter.Equals(':')) && (message.Count() == i + 1 || message[i + 1].Equals(' ') || message[i + 1].Equals('\n'))) // If we reach a quotation mark or colon and it's the end of the message / sentence
                    {
                        Thread.Sleep(110);
                    }
                    else if (letter.Equals(']') && (message.Count() == i + 1 || message[i + 1].Equals(' '))) // If we reach a closing square bracket and it's the end of the message / sentence
                    {
                        Thread.Sleep(130);
                    }
                    else
                    {
                        Thread.Sleep(30);
                    }

                    if (message.Count() == i + 1)
                    {
                        Console.WriteLine();
                        Thread.Sleep(80);
                    }

                    i += 1;
                }


            }
#endif
        }

        /// <summary>
        /// Writes a message to the console with delays between each letter
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="ms">The time to wait in milliseconds between each character.</param>
        public void WriteLine(string message, int ms)
        {
#if DEBUG
            Console.WriteLine(message);
#else
            int i = 0;
            foreach (var letter in message)
            {
                Console.Write(letter);
                if (letter.Equals('.') && (message.Count() == i + 1 || message[i + 1].Equals(' '))) // If we reach a full stop and it's the last in the message / sentence.
                {
                    Thread.Sleep(1000);
                }
                else if (letter.Equals('"') && (message.Count() == i + 1 || message[i + 1].Equals(' '))) // If we reach a quotation mark and it's the end of the message / sentence
                {
                    Thread.Sleep(1200);
                }
                else if (letter.Equals(']') && (message.Count() == i + 1 || message[i + 1].Equals(' '))) // If we reach a closing square bracket and it's the end of the message / sentence
                {
                    Thread.Sleep(1500);
                }
                else
                {
                    Thread.Sleep(ms);
                }

                if (message.Count() == i + 1)
                {
                    Console.WriteLine();
                    Thread.Sleep(1000);
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
            Console.WriteLine($"[{type}]: {message}");
#endif


            Console.ResetColor();
        }
    }
}
