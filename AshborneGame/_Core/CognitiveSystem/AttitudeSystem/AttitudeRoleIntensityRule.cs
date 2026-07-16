using AshborneGame._Core.CognitiveSystem.MemorySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.CognitiveSystem.AttitudeSystem
{
    public record AttitudeRoleIntensityRule
    {
        public MemoryRole Role { get; init; }

        /// <summary>
        /// How strongly this role is affected by the attitude (-1 to 1).
        /// </summary>
        public double Intensity { get; init; }

        public AttitudeRoleIntensityRule(MemoryRole role, double intensity)
        {
            Role = role;
            Intensity = intensity;
        }
    }
}