using AshborneGame._Core.Data.IDSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AshborneGame._Core.SaveSystem.Data
{
    public sealed class GameStateSaveData
    {
        public Dictionary<string, bool> Flags { get; set; } = new();
        public Dictionary<string, int> Counters { get; set; } = new();
        public Dictionary<string, string> Labels { get; set; } = new();
        public Dictionary<string, JsonElement> Variables { get; set; } = new(); // typed JSON values
        public Dictionary<string, InstanceID> Masks { get; set; } = new(); // mask name -> BOCSObject InstanceId
        public TimeTrackerSaveData TimeTracker { get; set; } = null!;
        // TODO: revamp quest system to be more data-driven and saveable
    }
}
