using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.IDSystem
{
    public readonly record struct InstanceID(Guid Value)
    {
        public static InstanceID New()
            => new(Guid.NewGuid());
    }
}