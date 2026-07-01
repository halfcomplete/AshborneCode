using AshborneGame._Core.CognitiveSystem.MemorySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Game.Events
{
    public static class ExternalEventBuilder
    {
        public static string EventName { get; private set; } = "";
        public static Dictionary<string, List<MemoryRole>> EventParticipants { get; private set; } = new();
        public static Dictionary<string, string> EventData { get; private set; } = new();

        public static void BeginNew(string eventName)
        {
            EventName = eventName;
        }

        public static void AddParticipant(string participantId, List<MemoryRole> memoryRoles)
        {
            EventParticipants[participantId] = memoryRoles;
        }

        public static void AddData(string dataName, string dataValue)
        {
            EventData[dataName] = dataValue;
        }

        public static void Commit()
        {

        }

        public static void Clear()
        {
            EventName = "";
            EventParticipants = new();
            EventData = new();
        }
    }
}
