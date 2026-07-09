using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.CognitiveSystem;

namespace AshborneGame._Core.CognitiveSystem.EmotionSystem
{
    /// <summary>
    /// Represents who this NPC is. Generally stable over time. Affects how a particular NPC reacts to certain MemoryTags and a Memory's final intensity.
    /// </summary>
    public class PersonalityProfile
    {
        /// <summary>
        /// A dictionary where the Key is a PersonalityTrait and the value is a double from 0 to 1 tracking how much of this personality trait is in this NPC.
        /// </summary>
        public Dictionary<PersonalityTrait, double> PersonalityTraits { get; } = [];
        
        public PersonalityProfile(Dictionary<PersonalityTrait, double> personalityTraits)
        {
            PersonalityTraits = personalityTraits;
        }

        public PersonalityProfile()
        {
            foreach (var trait in Enum.GetValues<PersonalityTrait>())
            {
                PersonalityTraits.Add(trait, 0.0);
            }
        }


        public void LoadSaveData(PersonalityProfile saveData)
        {
            PersonalityTraits.Clear();
            foreach (var trait in saveData.PersonalityTraits)
            {
                PersonalityTraits.Add(trait.Key, trait.Value);
            }
        }
    }
}