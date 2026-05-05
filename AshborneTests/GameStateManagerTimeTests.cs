using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core._Player;
using Xunit;

namespace AshborneTests
{
    /// <summary>
    /// Unit tests for the GameStateManager's discrete In-Game Time capabilities.
    /// </summary>
    [Collection("AshborneTests")]
    public class GameStateManagerTimeTests
    {
        private GameStateManager CreateManager()
        {
            var player = new Player();
            return new GameStateManager(player);
        }

        [Fact]
        public void AdvanceTime_ShouldIncreaseTotalInGameHours()
        {
            // Arrange
            var manager = CreateManager();
            int initialHours = manager.TotalInGameHours; // Usually 6 by default

            // Act
            manager.AdvanceTime(5);

            // Assert
            Assert.Equal(initialHours + 5, manager.TotalInGameHours);
        }
        
        [Fact]
        public void AdvanceTime_ShouldNotDecreaseHours_WhenGivenNegativeValue()
        {
            // Arrange
            var manager = CreateManager();
            int initialHours = manager.TotalInGameHours;

            // Act
            manager.AdvanceTime(-3);

            // Assert
            Assert.Equal(initialHours, manager.TotalInGameHours);
        }

        [Theory]
        [InlineData(6, TimeOfDay.Dawn)]
        [InlineData(7, TimeOfDay.Dawn)]
        [InlineData(8, TimeOfDay.Morning)]
        [InlineData(11, TimeOfDay.Morning)]
        [InlineData(12, TimeOfDay.Midday)]
        [InlineData(13, TimeOfDay.Midday)]
        [InlineData(14, TimeOfDay.Afternoon)]
        [InlineData(17, TimeOfDay.Afternoon)]
        [InlineData(18, TimeOfDay.Dusk)]
        [InlineData(19, TimeOfDay.Dusk)]
        [InlineData(20, TimeOfDay.Night)]
        [InlineData(23, TimeOfDay.Night)]
        [InlineData(0, TimeOfDay.Midnight)]
        [InlineData(4, TimeOfDay.Midnight)]
        public void CurrentTimeOfDay_ShouldReturnCorrectEnum_ForGivenHour(int hourOfDay, TimeOfDay expectedTime)
        {
            // Arrange
            var manager = CreateManager();
            
            // Fast-forward to a clean slate starting at hour 0 (Midnight of a new day)
            int hoursToMidnight = 24 - (manager.TotalInGameHours % 24);
            manager.AdvanceTime(hoursToMidnight);
            
            // Advance to target hour
            manager.AdvanceTime(hourOfDay);

            // Act
            var actualTime = manager.CurrentTimeOfDay;

            // Assert
            Assert.Equal(expectedTime, actualTime);
        }

        [Fact]
        public void AdvanceToTimeOfDay_ShouldJumpToNextOccurrence()
        {
            // Arrange
            var manager = CreateManager();
            // Start at Dawn (Hour 6)
            int startingHours = manager.TotalInGameHours;
            
            // Act: Advance to Dusk (Hour 18)
            manager.AdvanceToTimeOfDay(TimeOfDay.Dusk);

            // Assert
            Assert.Equal(TimeOfDay.Dusk, manager.CurrentTimeOfDay);
            Assert.Equal(startingHours + 12, manager.TotalInGameHours); // 6 + 12 = 18
        }
        
        [Fact]
        public void AdvanceToTimeOfDay_ShouldJumpToNextDay_IfTargetTimeIsEarlierInDay()
        {
            // Arrange
            var manager = CreateManager();
            
            // Set time to Night (Hour 21)
            manager.AdvanceToTimeOfDay(TimeOfDay.Night);
            int nightHours = manager.TotalInGameHours;

            // Act: Advance to Morning (Hour 9)
            manager.AdvanceToTimeOfDay(TimeOfDay.Morning);

            // Assert
            Assert.Equal(TimeOfDay.Morning, manager.CurrentTimeOfDay);
            
            // Since it was hour 21, leaping to hour 9 of the NEXT day takes 12 hours (21 + 3 hours to midnight + 9 hours to Morning)
            Assert.Equal(nightHours + 12, manager.TotalInGameHours); 
        }

        [Fact]
        public void TotalInGameHours_ShouldPersistCorrectly_OverManyDays()
        {
            // Arrange
            var manager = CreateManager();
            
            // Act - simulate waiting 3 full days plus 5 hours
            manager.AdvanceTime((24 * 3) + 5);
            
            // Assert
            Assert.Equal(6 + 77, manager.TotalInGameHours); // Start 6 + 77 = 83.
            Assert.Equal(TimeOfDay.Morning, manager.CurrentTimeOfDay); // 83 % 24 = 11 (Morning)
        }
    }
}