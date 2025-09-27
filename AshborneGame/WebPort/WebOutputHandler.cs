using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;

namespace AshborneGame.WebPort
{
    public class WebOutputHandler : IOutputHandler
    {
        private readonly Func<string, Task> _writeDialogueLineCallback;
        private readonly Func<string, Task> _writeNonDialogueLineCallback;
        private readonly Func<ConsoleMessageTypes, string, Task> _debugCallback;

        public WebOutputHandler(
            Func<string, Task> writeDialogueLineCallback,
            Func<string, Task> writeNonDialogueLineCallback,
            Func<ConsoleMessageTypes, string, Task>? debugCallback = null)
        {
            _writeDialogueLineCallback = writeDialogueLineCallback;
            _writeNonDialogueLineCallback = writeNonDialogueLineCallback;
            _debugCallback = debugCallback ?? ((type, msg) => Task.CompletedTask);
        }

        public void Write(string message)
        {
            // For now, just treat it as dialogue, and write it as a separate line.
            _writeDialogueLineCallback(message).GetAwaiter().GetResult();
        }

        public void Write(string message, int ms)
        {
            // For now, just treat it as dialogue, and write it as a separate line.
            _writeDialogueLineCallback(message).GetAwaiter().GetResult();
        }

        public void WriteNonDialogueLine(string message)
        {
            _writeNonDialogueLineCallback(message).GetAwaiter().GetResult();
        }

        public void WriteNonDialogueLine(string message, int ms)
        {
            // Ignoring delay for web output
            _writeNonDialogueLineCallback(message).GetAwaiter().GetResult();
        }

        public void WriteDialogueLine(string message)
        {
            _writeDialogueLineCallback(message).GetAwaiter().GetResult();
        }

        public void WriteDialogueLine(string message, int ms)
        {
            // Typewriter output
            _writeDialogueLineCallback($"{OutputConstants.DefaultTypeSpeed * OutputConstants.TypeSpeedMultiplier}{OutputConstants.TypewriterStartMarker}{message}{OutputConstants.TypewriterEndMarker}").GetAwaiter().GetResult();
        }

        public void DisplayFailMessage(string message)
        {
            _debugCallback(ConsoleMessageTypes.ERROR, message).GetAwaiter().GetResult();
        }

        public void DisplayDebugMessage(string message, ConsoleMessageTypes type = ConsoleMessageTypes.INFO)
        {
#if DEBUG
            _debugCallback(type, message).GetAwaiter().GetResult();
#endif
        }
    }
}
