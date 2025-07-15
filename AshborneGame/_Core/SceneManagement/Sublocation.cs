using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Interfaces;

namespace AshborneGame._Core.SceneManagement
{
    /// <summary>
    /// Represents a sublocation within a location that contains an interactive object.
    /// </summary>
    public class Sublocation : ILocation
    {
        private readonly Location _locationImpl;
        public Location ParentLocation { get; }
        public BOCSGameObject Object { get; }

        public Sublocation(Location parentLocation, BOCSGameObject @object, string name, string description, int minimumVisibility)
        {
            _locationImpl = new Location(name, description, minimumVisibility);
            ParentLocation = parentLocation;
            Object = @object;
        }

        // ILocation implementation (delegated to _locationImpl)
        public string Id => _locationImpl.Id;
        public string Name => _locationImpl.Name;
        public string ReferenceName => _locationImpl.ReferenceName;
        public string Description => _locationImpl.Description;
        public string AdditionalDescription => _locationImpl.AdditionalDescription;
        public string PositionalPrefix => _locationImpl.PositionalPrefix;
        public Dictionary<string, (Func<string> message, Action effect)> CustomCommands => _locationImpl.CustomCommands;
        public IReadOnlyDictionary<string, Location> Exits => _locationImpl.Exits;
        public IReadOnlyList<Sublocation> Sublocations => _locationImpl.Sublocations;
        public List<(Func<GameStateManager, bool> condition, string description)> Conditions => _locationImpl.Conditions;
        public int TimesVisited { get => _locationImpl.TimesVisited; set => _locationImpl.TimesVisited = value; }
        public void AddExit(string direction, Location location) => _locationImpl.AddExit(direction, location);
        public void AddSublocation(Sublocation sublocation) => _locationImpl.AddSublocation(sublocation);
        public string GetExits(Player player) => _locationImpl.GetExits(player);
        public string GetSublocations(Player player) => _locationImpl.GetSublocations(player);
        public void AddCustomCommand(List<string> commands, Func<string> messageFunc, Action effect) => _locationImpl.AddCustomCommand(commands, messageFunc, effect);
        public bool CanPlayerSeeExit(Player player) => _locationImpl.CanPlayerSeeExit(player);
        public void AddCondition(Func<GameStateManager, bool> condition, string description) => _locationImpl.AddCondition(condition, description);
        public string GetDescription(Player player, GameStateManager state) => _locationImpl.GetDescription(player, state);
        public string GetFullDescription(Player player) => _locationImpl.GetFullDescription(player);

        public bool CanPlayerSeeSublocation(Player player) => CanPlayerSeeExit(player);
    }
}
