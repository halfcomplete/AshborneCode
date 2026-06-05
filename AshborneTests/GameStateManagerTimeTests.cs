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
        private TimeTracker CreateTimeTracker()
        {
            var questTracker = new QuestTracker();
            return new TimeTracker(questTracker);
        }

        [Fact]
        public void AdvanceTime_ShouldIncreaseTotalInGameHours()
        {
            // Arrange
            var timeTracker = CreateTimeTracker();
            int initialHours = timeTracker.TotalInGameHours; // Usually 6 by default

            // Act
            timeTracker.AdvanceTime(5);

            // Assert
            Assert.Equal(initialHours + 5, timeTracker.TotalInGameHours);
        }
        
        [Fact]
        public void AdvanceTime_ShouldNotDecreaseHours_WhenGivenNegativeValue()
        {
            // Arrange
            var timeTracker = CreateTimeTracker();
            int initialHours = timeTracker.TotalInGameHours;

            // Act
            timeTracker.AdvanceTime(-3);

            // Assert
            Assert.Equal(initialHours, timeTracker.TotalInGameHours);
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
            var timeTracker = CreateTimeTracker();
            
            // Fast-forward to a clean slate starting at hour 0 (Midnight of a new day)
            int hoursToMidnight = 24 - (timeTracker.TotalInGameHours % 24);
            timeTracker.AdvanceTime(hoursToMidnight);
            
            // Advance to target hour
            timeTracker.AdvanceTime(hourOfDay);

            // Act
            var actualTime = timeTracker.CurrentTimeOfDay;

            // Assert
            Assert.Equal(expectedTime, actualTime);
        }

        [Fact]
        public void AdvanceToTimeOfDay_ShouldJumpToNextOccurrence()
        {
            // Arrange
            var timeTracker = CreateTimeTracker();
            // Start at Dawn (Hour 6)
            int startingHours = timeTracker.TotalInGameHours;
            
            // Act: Advance to Dusk (Hour 18)
            timeTracker.AdvanceToTimeOfDay(TimeOfDay.Dusk);

            // Assert
            Assert.Equal(TimeOfDay.Dusk, timeTracker.CurrentTimeOfDay);
            Assert.Equal(startingHours + 12, timeTracker.TotalInGameHours); // 6 + 12 = 18
        }
        
        [Fact]
        public void AdvanceToTimeOfDay_ShouldJumpToNextDay_IfTargetTimeIsEarlierInDay()
        {
            // Arrange
            var timeTracker = CreateTimeTracker();
            
            // Set time to Night (Hour 21)
            timeTracker.AdvanceToTimeOfDay(TimeOfDay.Night);
            int nightHours = timeTracker.TotalInGameHours;

            // Act: Advance to Morning (Hour 9)
            timeTracker.AdvanceToTimeOfDay(TimeOfDay.Morning);

            // Assert
            Assert.Equal(TimeOfDay.Morning, timeTracker.CurrentTimeOfDay);
            
            // Since it was hour 21, leaping to hour 9 of the NEXT day takes 12 hours (21 + 3 hours to midnight + 9 hours to Morning)
            Assert.Equal(nightHours + 12, timeTracker.TotalInGameHours); 
        }

        [Fact]
        public void TotalInGameHours_ShouldPersistCorrectly_OverManyDays()
        {
            // Arrange
            var timeTracker = CreateTimeTracker();
            
            // Act - simulate waiting 3 full days plus 5 hours
            timeTracker.AdvanceTime((24 * 3) + 5);
            
            // Assert
            Assert.Equal(6 + 77, timeTracker.TotalInGameHours); // Start 6 + 77 = 83.
            Assert.Equal(TimeOfDay.Morning, timeTracker.CurrentTimeOfDay); // 83 % 24 = 11 (Morning)
        }
    }
}