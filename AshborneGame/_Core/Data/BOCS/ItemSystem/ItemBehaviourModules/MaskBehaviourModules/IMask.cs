using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules.MaskBehaviourModules
{
    internal interface IMask
    {
        bool TryGetInterjection(out string interjection);
    }
}
