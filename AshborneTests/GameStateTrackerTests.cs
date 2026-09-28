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
            var flagKey = GameStateTracker.ValidateAndGetFlagKey("Flags.Player.Received.OssanethMask");
            var counterKey = GameStateTracker.ValidateAndGetCounterKey("Counters.Player.Prayers");
            var labelKey = GameStateTracker.ValidateAndGetLabelKey("Labels.Player.Name");

            Assert.True(tracker.TryGetFlag(flagKey, out var defaultFlag));
            Assert.True(tracker.TryGetCounter(counterKey, out var defaultValue));
            Assert.Equal(string.Empty, tracker.TryGetLabel(labelKey));
            Assert.False(defaultFlag);
            Assert.Equal(0, defaultValue);

            tracker.SetCounter(counterKey, 2);

            Assert.True(tracker.TryIncrementCounter(counterKey, 3));
            Assert.True(tracker.TryGetCounter(counterKey, out var value));
            Assert.Equal(5, value);
        }

        [Fact]
        public void Tracker_RemoveOperationsResetValuesWithoutRemovingRegisteredState()
        {
            var tracker = new GameStateTracker();
            var flagKey = GameStateTracker.ValidateAndGetFlagKey("Flags.Player.Received.OssanethMask");
            var counterKey = GameStateTracker.ValidateAndGetCounterKey("Counters.Player.Prayers");
            var labelKey = GameStateTracker.ValidateAndGetLabelKey("Labels.Player.Name");

            tracker.SetFlag(flagKey, true);
            tracker.SetCounter(counterKey, 4);
            tracker.SetLabel(labelKey, "Hero");

            Assert.True(tracker.RemoveFlag(flagKey));
            Assert.True(tracker.RemoveCounter(counterKey));
            Assert.True(tracker.RemoveLabel(labelKey));
            Assert.True(tracker.TryGetFlag(flagKey, out var flagValue));
            Assert.True(tracker.TryGetCounter(counterKey, out var counterValue));
            Assert.Equal(string.Empty, tracker.TryGetLabel(labelKey));
            Assert.False(flagValue);
            Assert.Equal(0, counterValue);
        }
    }
}
