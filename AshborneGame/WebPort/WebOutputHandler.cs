using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;

namespace AshborneGame.WebPort
{
    public class WebOutputHandler : IOutputHandler
    {
        private readonly Func<string, Task> _writeLineCallback;
        private readonly Func<Task> _clearCallback;
        private readonly Func<ConsoleMessageTypes, string, Task> _debugCallback;

        public WebOutputHandler(
            Func<string, Task> writeLineCallback,
            Func<Task> clearCallback,
            Func<ConsoleMessageTypes, string, Task>? debugCallback = null)
        {
            _writeLineCallback = writeLineCallback;
            _clearCallback = clearCallback;
            _debugCallback = debugCallback ?? ((type, msg) => Task.CompletedTask);
        }

        public void Write(string message)
        {
            // Write without newline
            _writeLineCallback?.Invoke(message);
        }

        public async void Write(string message, int ms)
        {
            await _writeLineCallback?.Invoke($"{ms}__TYPEWRITER_START__{message}__TYPEWRITER_END__");
        }

        public async void WriteLine(string message)
        {
            if (message.TrimEnd().EndsWith("__PAUSE__"))
            {
                // Handle special pause marker: ms__PAUSE__
                await _writeLineCallback?.Invoke($"{message}");
                return;
            }
            await _writeLineCallback?.Invoke($"__TYPEWRITER_START__{message}__TYPEWRITER_END__\n");
        }

        public async void WriteLine(string message, int ms)
        {
            await _writeLineCallback?.Invoke($"{ms}__TYPEWRITER_START__{message}__TYPEWRITER_END__\n");
        }

        public void DisplayFailMessage(string message)
        {
            // Later, display a fail message on UI
            // E.g., using a toast notification or alert
            // For now, just log it
            _writeLineCallback?.Invoke($"[FAIL] {message}\n");
        }

        public void DisplayDebugMessage(string message, ConsoleMessageTypes type)
        {
#if DEBUG
            _debugCallback?.Invoke(type, message);
#endif
        }

        public void Clear()
        {
            _clearCallback?.Invoke();
        }
    }
}
