using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Game.DescriptionHandling
{
    public class SensoryDescription
    {
        public string Visual { get; private set; }
        public string Auditory { get; private set; }
        public string? Tactile { get; private set; }
        public string? Olfactory { get; private set; }
        public string? Gustatory { get; private set; }

        public SensoryDescription(
            string visual,
            string auditory,
            string? tactile = null,
            string? olfactory = null,
            string? gustatory = null)
        {
            Visual = visual;
            Auditory = auditory;
            Tactile = tactile;
            Olfactory = olfactory;
            Gustatory = gustatory;
        }

        public SensoryDescription()
        {
            Visual = string.Empty;
            Auditory = string.Empty;
            Tactile = null;
            Olfactory = null;
            Gustatory = null;
        }

        public string GetRandomDescription()
        {
            List<string> availableDescriptions = [];

            if (!string.IsNullOrEmpty(Visual))
                availableDescriptions.Add(Visual);

            if (!string.IsNullOrEmpty(Auditory))
                availableDescriptions.Add(Auditory);

            if (!string.IsNullOrEmpty(Tactile))
                availableDescriptions.Add(Tactile!);

            if (!string.IsNullOrEmpty(Olfactory))
                availableDescriptions.Add(Olfactory!);

            if (!string.IsNullOrEmpty(Gustatory))
                availableDescriptions.Add(Gustatory!);

            if (availableDescriptions.Count == 0)
                return string.Empty;
            
            int descriptionCount = availableDescriptions.Count;
            if (descriptionCount == 0)
                return string.Empty;
            var random = new Random();
            int index = random.Next(descriptionCount);
            return availableDescriptions[index];
        }
    }
}
