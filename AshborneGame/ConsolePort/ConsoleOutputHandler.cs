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
            // For console, ignore web typewriter markers and use native typewriter effect
            if (message.Contains("__TYPEWRITER_START__") && message.Contains("__TYPEWRITER_END__"))
            {
                // Extract the actual message and use regular WriteLine (which has built-in typewriter effect)
                int startIndex = message.IndexOf("__TYPEWRITER_START__") + "__TYPEWRITER_START__".Length;
                int endIndex = message.IndexOf("__TYPEWRITER_END__");
                string actualMessage = message.Substring(startIndex, endIndex - startIndex);
                
                WriteLine(actualMessage); // Use the regular WriteLine with built-in typewriter effect
                return;
            }

            // Use the built-in typewriter effect from the regular WriteLine method
            WriteLine(message);
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
    }
}
