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
            // Type = Fear, Amount = 50, Intensity = 2, StartHour = 10
            var modifier = new EmotionModifier(EmotionTypes.Fear, 50, 2, 10);

            // Act & Assert
            // At hour 10 (0 elapsed) => amountDecayed = 0 => 50
            Assert.Equal(50, modifier.GetCurrentAmount(10));

            // At hour 12 (2 elapsed) => amountDecayed = 1 => 49
            Assert.Equal(49, modifier.GetCurrentAmount(12));

            // At hour 20 (10 elapsed) => amountDecayed = 5 => 45
            Assert.Equal(45, modifier.GetCurrentAmount(20));
        }

        [Fact]
        public void GetCurrentAmount_ShouldNotDecayBelowZero_WhenPositiveOrNegative()
        {
            // Arrange (Positive)
            var positiveMod = new EmotionModifier(EmotionTypes.Happiness, 10, 1, 10);
            
            // Expected to hit 0 at hour 20 (10 elapsed / intensity 1 = 10 decay)
            Assert.Equal(0, positiveMod.GetCurrentAmount(21));

            // Arrange (Negative)
            var negativeMod = new EmotionModifier(EmotionTypes.Sadness, -10, 1, 10);
            
            // Expected to hit 0 at hour 20
            Assert.Equal(0, negativeMod.GetCurrentAmount(21));
        }

        [Fact]
        public void IsDepleted_ShouldReturnTrue_WhenValueReachesZero()
        {
            // Arrange
            var modifier = new EmotionModifier(EmotionTypes.Anger, 5, 2, 10);

            // Act & Assert
            Assert.False(modifier.IsDepleted(10)); // Current 5
            Assert.False(modifier.IsDepleted(18)); // Current 1 (8 hours / 2 = 4 decay)
            Assert.True(modifier.IsDepleted(20));  // Current 0 (10 hours / 2 = 5 decay)
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
            profile.AddModifier(new EmotionModifier(EmotionTypes.Fear, 20, 2, 5));
            profile.AddModifier(new EmotionModifier(EmotionTypes.Fear, 30, 1, 5));
            profile.AddModifier(new EmotionModifier(EmotionTypes.Happiness, 50, 1, 5)); // Different type

            // Act - Evaluate at Hour 7
            // Mod 1: 20 - (2/2) = 19
            // Mod 2: 30 - (2/1) = 28
            // Total Fear = 19 + 28 = 47
            int fearResult = profile.GetCurrentEmotion(EmotionTypes.Fear, 7);
            
            // Total Happiness: 50 - (2/1) = 48
            int happyResult = profile.GetCurrentEmotion(EmotionTypes.Happiness, 7);

            // Assert
            Assert.Equal(47, fearResult);
            Assert.Equal(48, happyResult);
        }

        [Fact]
        public void GetCurrentEmotion_ShouldClampEmotionsTo100()
        {
            // Arrange
            var profile = new EmotionProfile();
            
            // Total = 150
            profile.AddModifier(new EmotionModifier(EmotionTypes.Disgust, 80, 999, 1));
            profile.AddModifier(new EmotionModifier(EmotionTypes.Disgust, 70, 999, 1));

            // Act
            int result = profile.GetCurrentEmotion(EmotionTypes.Disgust, 1);

            // Assert
            Assert.Equal(100, result);
        }
        
        [Fact]
        public void GetCurrentEmotion_ShouldClampEmotionsToZero()
        {
            // Arrange
            var profile = new EmotionProfile();
            
            // Total = -10
            profile.AddModifier(new EmotionModifier(EmotionTypes.Sadness, -10, 999, 1));

            // Act
            int result = profile.GetCurrentEmotion(EmotionTypes.Sadness, 1);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetCurrentEmotion_ShouldRemoveDepletedModifiers_WhenEvaluated()
        {
            // Arrange
            var profile = new EmotionProfile();
            
            // Initial Amount = 5, Intensity = 1, StartHour = 1
            // Will be depleted by Hour 6
            profile.AddModifier(new EmotionModifier(EmotionTypes.Surprise, 5, 1, 1));
            
            // Act: Evaluate at hour 10 (guaranteed depleted)
            int result = profile.GetCurrentEmotion(EmotionTypes.Surprise, 10);

            // Assert
            Assert.Equal(0, result);
            
            // Add a new modifier to check if old ones were cleaned out
            // If they weren't cleaned out, this might mess up counts if the old ones had weird boundaries, 
            // but GetCurrentEmotion implicitly removes them via CleanUpDepleted().
            profile.AddModifier(new EmotionModifier(EmotionTypes.Surprise, 10, 1, 10));
            int newResult = profile.GetCurrentEmotion(EmotionTypes.Surprise, 10);
            
            Assert.Equal(10, newResult);
        }

        #endregion
    }
}