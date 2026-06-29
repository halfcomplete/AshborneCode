
namespace AshborneGame._Core.CognitiveSystem.EmotionSystem
{
    /// <summary>
    /// Defines the primary human emotion types used for character states and interactions.
    /// </summary>
    /// <remarks>
    /// These are not meant to describe "feelings" towards specific objects or people, but rather the core emotional states that characters experience
    /// moment-to-moment. Later, the EmotionSystem can be expanded to track specific feelings towards objects, people, or events by associating these core emotions with specific targets.
    /// Emotions can be combined to create more complex emotional states, and their intensities can vary to reflect different levels of emotional response.
    /// <br>For example:</br>
    /// <list type="bullet">
    /// <item>Surprise + Happiness = Excitement</item>
    /// <item>Surprise + Fear = Shock</item>
    /// <item>Contempt + Disgust = Disdain</item>
    /// </list>
    /// </remarks>
    public enum EmotionType
    {
        /// <summary>
        /// An emotional state involving feelings of hostility, frustration, and antagonism, often triggered when boundaries are violated.
        /// </summary>
        Anger,
        /// <summary>
        /// A complex, often social emotion involving a sense of superiority or disrespect, considered a blend of disgust and anger.
        /// </summary>
        Contempt,
        /// <summary>
        /// A feeling of revulsion or intense dislike, typically triggered by unpleasant sights, smells, or tastes, acting as a protective mechanism.
        /// </summary>
        Disgust,
        /// <summary>
        /// A survival-based response to perceived danger or threat, triggering a "fight-or-flight" reaction.
        /// </summary>
        Fear,
        /// <summary>
        /// A pleasant state associated with joy, contentment, and satisfaction. Happiness may also arise when reflecting upon the past.
        /// </summary>
        Happiness,
        /// <summary>
        /// A state of feeling sorrow, grief, or disappointment, often arising from loss. Sadness may also arise when reflecting upon the past.
        /// </summary>
        Sadness,
        /// <summary>
        /// A brief, intense response to unexpected events, which can be positive or negative.
        /// </summary>
        Surprise
    }
}
