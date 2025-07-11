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
            // Simple newline version
            _writeCallback?.Invoke(message + "\n");
        }

        public async void WriteLine(string message, int ms)
        {
            // Simulate typewriter-style pacing
            int i = 0;
            while (i < message.Length)
            {
                char c = message[i];
                await _writeCallback?.Invoke(c.ToString());

                if ((c == '.' || c == ':' || c == '"' || c == ']') &&
                    (i == message.Length - 1 || message[i + 1] == ' ' || message[i + 1] == '\n'))
                {
                    await Task.Delay(110);
                }
                else
                {
                    await Task.Delay(ms);
                }

                i++;
            }

            await _writeCallback?.Invoke("\n");
            await Task.Delay(200);
        }

        public void DisplayDebugMessage(string message, ConsoleMessageTypes type)
        {
            _debugCallback?.Invoke(type, message);
        }

        public void Clear()
        {
            _clearCallback?.Invoke();
        }
    }
}
