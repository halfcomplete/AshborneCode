using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.Globals.Interfaces
{
    public interface IOutputHandler
    {
        Task Write(string message);
        Task Write(string message, int ms);
        Task WriteNonDialogueLine(string message);
        Task WriteNonDialogueLine(string message, int ms);
        Task WriteDialogueMarkerLine(string message);
        Task WriteDialogueLine(string message, int ms);
        Task DisplayFailMessage(string message);
        Task DisplayDebugMessage(string message, ConsoleMessageTypes type = ConsoleMessageTypes.INFO);
    }
}
