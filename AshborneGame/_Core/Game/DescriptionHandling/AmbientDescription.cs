using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Game.DescriptionHandling
{
    public class AmbientDescription
    {
        public Dictionary<TimeSpan, string> FromDuration { get; private set; }
        public List<string>? FromRandom { get; private set; }

        public AmbientDescription(Dictionary<TimeSpan, string> fromDuration, List<string>? fromRandom = null)
        {
            FromDuration = fromDuration;
            FromRandom = fromRandom;
        }

        public string GetSnippetFromRandom()
        {
            return FromRandom != null && FromRandom.Count > 0
                ? FromRandom[new Random().Next(FromRandom.Count)]
                : string.Empty;
        }
    }
}
