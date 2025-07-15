using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;

namespace AshborneGame.WebPort
{
    public class WebOutputHandler : IOutputHandler
    {
        private readonly Func<string, Task> _writeCallback;
        private readonly Func<Task> _clearCallback;
        private readonly Func<ConsoleMessageTypes, string, Task> _debugCallback;

        public WebOutputHandler(
            Func<string, Task> writeCallback,
            Func<Task> clearCallback,
            Func<ConsoleMessageTypes, string, Task>? debugCallback = null)
        {
            _writeCallback = writeCallback;
            _clearCallback = clearCallback;
            _debugCallback = debugCallback ?? ((type, msg) => Task.CompletedTask);
        }

        public void Write(string message)
        {
            // Write without newline
            _writeCallback?.Invoke(message);
        }

        public void WriteLine(string message)
        {
            // Simple newline version - no buffering
            _writeCallback?.Invoke(message + "\n");
        }

        public async void WriteLine(string message, int ms)
        {
            await _writeCallback?.Invoke($"__TYPEWRITER_START__{message}__TYPEWRITER_END__\n");
        }

        public void DisplayFailMessage(string message)
        {
            // Later, display a fail message on UI
            // E.g., using a toast notification or alert
            // For now, just log it
            _writeCallback?.Invoke($"[FAIL] {message}\n");
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
