using AshborneGame._Core.CognitiveSystem.AttitudeSystem;
using System.ComponentModel;

namespace AshborneGame._Core.Globals.Services
{
    public class AttitudeToAdjective
    {
        public static string GetAttitudeDescriptor(AttitudeFactor factor)
        {
            return factor switch
            {
                AttitudeFactor.Affection => "like",
                AttitudeFactor.Dominance => "dominate",
                AttitudeFactor.Fear => "fear",
                AttitudeFactor.Respect => "respect",
                AttitudeFactor.Trust => "trust",
                _ => throw new InvalidEnumArgumentException($"Unexpected AttitudeFactor value: {factor}")
            };
        }
    }
}
