using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCCapabilities
{
    public interface ICanBeAttacked
    {
        double Health { get; set; }
        double MaxHealth { get; set; }
        void Attacked(double damage);
    }
}
