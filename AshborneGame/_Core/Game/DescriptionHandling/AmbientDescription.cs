using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Game.DescriptionHandling
{
    /// <summary>
    /// Timed and random ambient text snippets for a location.
    /// <see cref="FromDuration"/> keys are tick counts (1 tick = 1 second) that must elapse
    /// before the corresponding line is output.
    /// </summary>
    public class AmbientDescription
    {
        public Dictionary<int, string> FromDuration { get; private set; }
        public List<string> FromRandom { get; private set; }

        public AmbientDescription()
        {
            FromDuration = new();
            FromRandom = new();
        }

        public AmbientDescription(Dictionary<int, string> fromDuration, List<string>? fromRandom = null)
        {
            FromDuration = fromDuration;
            FromRandom = fromRandom ?? new();
        }

        public string GetSnippetFromRandom()
        {
            return FromRandom.Count > 0
                ? FromRandom[new Random().Next(FromRandom.Count)]
                : string.Empty;
        }

        public AmbientDescription AddTimeBased(int ticks, string desc)
        {
            FromDuration.Add(ticks, desc);
            return this;
        }

        public AmbientDescription AddRandom(string desc)
        {
            FromRandom.Add(desc);
            return this;
        }
    }
}
