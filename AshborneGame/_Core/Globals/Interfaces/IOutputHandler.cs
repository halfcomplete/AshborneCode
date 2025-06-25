using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.Globals.Interfaces
{
    public interface IOutputHandler
    {
        void Write(string message);
        void WriteLine(string message, int ms = 30);
        void DisplayDebugMessage(string message, ConsoleMessageTypes type = ConsoleMessageTypes.INFO);
    }
}
