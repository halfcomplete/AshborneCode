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

        // IOutputHandler.Write(string) implementation (void)
        public void Write(string message)
        {
            WriteAsync(message).GetAwaiter().GetResult();
        }

        // Task-based Write for internal async use
        public Task WriteAsync(string text)
        {
#if DEBUG && BLAZOR
            Console.WriteLine($"[DEBUG] WebOutputHandler.Write: {text}");
#endif
            return _writeLineCallback(text);
        }

        public async void Write(string message, int ms)
        {
            // Only apply non-dialogue output speed multiplier if not dialogue
            int adjustedMs = ms == AshborneGame._Core.Globals.Constants.OutputConstants.DefaultTypeSpeed ? ms : (int)(ms * AshborneGame._Core.Globals.Constants.OutputConstants.NonDialogueOutputSpeedMultiplier);
            await _writeLineCallback?.Invoke($"{adjustedMs}__TYPEWRITER_START__{message}__TYPEWRITER_END__");
        }

        // IOutputHandler.WriteLine(string) implementation (void)
        public void WriteLine(string message)
        {
            WriteLineAsync(message).GetAwaiter().GetResult();
        }

        // Task-based WriteLine for internal async use
        public Task WriteLineAsync(string line)
        {
#if DEBUG && BLAZOR
            Console.WriteLine($"[DEBUG] WebOutputHandler.WriteLine: {line}");
#endif
            return _writeLineCallback(line);
        }

        public async void WriteLine(string message, int ms)
        {
            // Only apply non-dialogue output speed multiplier if not dialogue
            int adjustedMs = ms == AshborneGame._Core.Globals.Constants.OutputConstants.DefaultTypeSpeed ? ms : (int)(ms * AshborneGame._Core.Globals.Constants.OutputConstants.NonDialogueOutputSpeedMultiplier);
            await _writeLineCallback?.Invoke($"{adjustedMs}__TYPEWRITER_START__{message}__TYPEWRITER_END__\n");
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
