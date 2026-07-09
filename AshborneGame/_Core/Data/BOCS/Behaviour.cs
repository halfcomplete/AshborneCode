using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.BOCS
{
    public abstract class Behaviour
    {
        public BOCSObject Owner { get; internal set; } = null!;

        public abstract Behaviour DeepClone();

        // Stable ID for save/load and migrations. Default: CLR full name.
        public virtual string SaveId => GetType().FullName!;

        // Return null to omit from save (stateless behaviours)
        public abstract BehaviourSaveData GetSaveData(SaveLoadContext context);

        public abstract void LoadSaveData(BehaviourSaveData data, SaveLoadContext context);
    }
}
