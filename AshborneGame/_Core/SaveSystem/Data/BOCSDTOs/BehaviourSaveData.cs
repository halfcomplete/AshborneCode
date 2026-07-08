using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AshborneGame._Core.SaveSystem.Data.BOCSDTOs
{
    public sealed class BehaviourSaveData
    {
        public string BehaviourId { get; set; } = null!;            // stable string key
        public JsonElement? State { get; set; }              // behaviour-owned payload

        public BehaviourSaveData(string behaviourId, JsonElement? state)
        {
            BehaviourId = behaviourId;
            State = state;
        }
    }
}
