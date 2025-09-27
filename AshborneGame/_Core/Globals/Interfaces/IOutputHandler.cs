using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.Globals.Interfaces
{
    public interface IOutputHandler
    {
        void Write(string message);
        void Write(string message, int ms);
        void WriteNonDialogueLine(string message);
        void WriteNonDialogueLine(string message, int ms);
        void WriteDialogueLine(string message);
        void WriteDialogueLine(string message, int ms);
        void DisplayFailMessage(string message);
        void DisplayDebugMessage(string message, ConsoleMessageTypes type = ConsoleMessageTypes.INFO);
    }
}
