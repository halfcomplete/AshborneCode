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
    }
}
