using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.MemorySystem
{
    public class MemoryDefinition
    {
        public double BaseIntensity { get; }

        public HashSet<MemoryTag> Tags { get; }

        public MemoryDefinition(double baseIntensity, HashSet<MemoryTag> tags)
        {
            BaseIntensity = baseIntensity;
            Tags = tags;
        }
    }
}