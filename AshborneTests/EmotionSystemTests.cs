using AshborneGame._Core.EmotionSystem;
using AshborneGame._Core.Globals.Enums;
using Xunit;

namespace AshborneTests
{
    /// <summary>
    /// Unit tests for the EmotionSystem, focusing on Lazy Evaluation of Emotion Modifiers
    /// and Profile aggregation over in-game time.
    /// </summary>
    [Collection("AshborneTests")]
    public class EmotionSystemTests
    {
        #region EmotionModifier Tests

        [Fact]
        public void GetCurrentAmount_ShouldDecayCorrectly_OverTime()
        {
            // Arrange
            // Type = Fear, Amount = 0.5, Intensity = 2, StartHour = 10
            var modifier = new EmotionModifier(EmotionType.Fear, 0.5f, 2, 10);

// Act & Assert
            // At hour 10 (0 elapsed) => amountDecayed = 0 => 0.5
            Assert.Equal(0.5f, modifier.GetCurrentAmount(10), 3);

            // At hour 12 (2 elapsed) => amountDecayed = 0.01 => 0.49
            Assert.Equal(0.49f, modifier.GetCurrentAmount(12), 3);

            // At hour 20 (10 elapsed) => amountDecayed = 0.05 => 0.45
            Assert.Equal(0.45f, modifier.GetCurrentAmount(20), 3);
        }

        [Fact]
        public void GetCurrentAmount_ShouldNotDecayBelowZero_WhenPositiveOrNegative()
        {
            // Arrange (Positive)
            var positiveMod = new EmotionModifier(EmotionType.Happiness, 0.10f, 1, 10);

            // Expected to hit 0 at hour 21 (11 elapsed / intensity 1 = 11 decay -> 0.11)
            Assert.Equal(0f, positiveMod.GetCurrentAmount(21), 3);

            // Arrange (Negative)
            var negativeMod = new EmotionModifier(EmotionType.Sadness, -0.10f, 1, 10);

            // Expected to hit 0 at hour 21
            Assert.Equal(0f, negativeMod.GetCurrentAmount(21), 3);
        }

        [Fact]
        public void IsDepleted_ShouldReturnTrue_WhenValueReachesZero()
        {
            // Arrange
            var modifier = new EmotionModifier(EmotionType.Anger, 0.05f, 2, 10);

            // Act & Assert
            Assert.False(modifier.IsDepleted(10)); // Current 0.05
            Assert.False(modifier.IsDepleted(18)); // Current 0.01 (8 hours / 2 = 4 -> 0.04)
            Assert.True(modifier.IsDepleted(20));  // Current 0 (10 hours / 2 = 5 -> 0.05)
            Assert.True(modifier.IsDepleted(100)); // Stays depleted
        }

        #endregion

        #region EmotionProfile Tests

        [Fact]
        public void GetCurrentEmotion_ShouldAggregateModifiersCorrectly()
        {
            // Arrange
            var profile = new EmotionProfile();

            // StartHour = 5.
            profile.AddModifier(new EmotionModifier(EmotionType.Fear, initialAmount: 0.20f, intensity: 2, startHour: 5));
            profile.AddModifier(new EmotionModifier(EmotionType.Fear, initialAmount: 0.30f, intensity: 1, startHour: 5));
            profile.AddModifier(new EmotionModifier(EmotionType.Happiness, initialAmount: 0.50f, intensity: 1, startHour: 5)); // Different type

            // Act - Evaluate at Hour 7
            // Mod 1: 0.20 - 0.01 = 0.19
            // Mod 2: 0.30 - 0.02 = 0.28
            // Total Fear = 0.19 + 0.28 = 0.47
            float fearResult = profile.GetCurrentEmotion(EmotionType.Fear, 7);

            // Total Happiness: 0.50 - 0.02 = 0.48
            float happyResult = profile.GetCurrentEmotion(EmotionType.Happiness, 7);

            // Assert
            Assert.Equal(0.47f, fearResult, 3);
            Assert.Equal(0.48f, happyResult, 3);
        }

        [Fact]
        public void GetCurrentEmotion_ShouldClampEmotionsTo100()
        {
            // Arrange
            var profile = new EmotionProfile();

            // Total = 1.5
            profile.AddModifier(new EmotionModifier(EmotionType.Disgust, 0.80f, 999, 1));
            profile.AddModifier(new EmotionModifier(EmotionType.Disgust, 0.70f, 999, 1));

            // Act
            float result = profile.GetCurrentEmotion(EmotionType.Disgust, 1);

            // Assert
            Assert.Equal(1f, result, 3);
        }

        [Fact]
        public void GetCurrentEmotion_ShouldClampEmotionsToZero()
        {
            // Arrange
            var profile = new EmotionProfile();

            // Total = -0.10
            profile.AddModifier(new EmotionModifier(EmotionType.Sadness, -0.10f, 999, 1));

            // Act
            float result = profile.GetCurrentEmotion(EmotionType.Sadness, 1);

            // Assert
            Assert.Equal(0f, result, 3);
        }

        [Fact]
        public void GetCurrentEmotion_ShouldRemoveDepletedModifiers_WhenEvaluated()
        {
            // Arrange
            var profile = new EmotionProfile();

            // Initial Amount = 0.05, Intensity = 1, StartHour = 1
            // Will be depleted by Hour 6
            profile.AddModifier(new EmotionModifier(EmotionType.Surprise, 0.05f, 1, 1));

            // Act: Evaluate at hour 10 (guaranteed depleted)
            float result = profile.GetCurrentEmotion(EmotionType.Surprise, 10);

            // Assert
            Assert.Equal(0f, result, 3);

            // Add a new modifier to check if old ones were cleaned out
            // If they weren't cleaned out, this might mess up counts if the old ones had weird boundaries, 
            // but GetCurrentEmotion implicitly removes them via CleanUpDepleted().
            profile.AddModifier(new EmotionModifier(EmotionType.Surprise, 0.10f, 1, 10));
            float newResult = profile.GetCurrentEmotion(EmotionType.Surprise, 10);

            Assert.Equal(0.10f, newResult, 3);
        }

        #endregion
    }
}
