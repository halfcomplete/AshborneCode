using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectCapabilities
{
    public interface IInteractable
    {
        void Interact(ObjectInteractionTypes _interaction, Player player);
    }
}
