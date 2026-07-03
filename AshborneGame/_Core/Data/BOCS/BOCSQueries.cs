using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.BOCS
{
    public static class BOCSQueries
    {
        public static bool IsItem(BOCSObject obj) => obj.HasBehaviours<ItemBehaviour>();

        // TODO: Make more exact
        public static bool IsNPC(BOCSObject obj) => obj.HasBehaviours<TalkableBehaviour>();

        public static bool IsObject(BOCSObject obj) => !(IsItem(obj) || IsNPC(obj));
    }
}
