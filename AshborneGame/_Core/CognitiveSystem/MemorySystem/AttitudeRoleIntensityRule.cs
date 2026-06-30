using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem
{
    public record AttitudeRoleIntensityRule
    {
        public MemoryRole Role { get; init; }

        /// <summary>
        /// How strongly this role is affected by the attitude (0 to 1).
        /// </summary>
        public double Weight { get; init; }

        public AttitudeRoleIntensityRule(MemoryRole role, double intensity)
        {
            Role = role;
            Weight = intensity;
        }
    }
}