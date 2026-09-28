using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Constants;

namespace AshborneTests
{
    [Collection("AshborneTests")]
    public class GameStateTrackerTests
    {
        [Fact]
        public void Tracker_UsesStateKeysAsTheSingleRegisteredCatalog()
        {
            Assert.Contains("Flags.Player.Received.OssanethMask", GameStateTracker.GetAllRegisteredFlagKeys());
            Assert.Contains("Counters.Player.Prayers", GameStateTracker.GetAllRegisteredCounterKeys());
            Assert.Contains("Labels.Player.Name", GameStateTracker.GetAllRegisteredLabelKeys());

            Assert.Equal(
                StateKeys.Flags.Player.Received.OssanethMask,
                GameStateTracker.ValidateAndGetFlagKey("Flags.Player.Received.OssanethMask"));
        }

        [Fact]
        public void Tracker_StoresAndValidatesLiveStateThroughTheSameKeyCatalog()
        {
            var tracker = new GameStateTracker();
            var counterKey = GameStateTracker.ValidateAndGetCounterKey("Counters.Player.Prayers");

            tracker.SetCounter(counterKey, 2);

            Assert.True(tracker.TryIncrementCounter(counterKey, 3));
            Assert.True(tracker.TryGetCounter(counterKey, out var value));
            Assert.Equal(5, value);
        }
    }
}
