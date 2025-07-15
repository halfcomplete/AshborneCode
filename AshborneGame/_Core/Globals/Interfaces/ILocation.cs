using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AshborneGame._Core.SceneManagement;
using AshborneGame._Core._Player;
using AshborneGame._Core.Game;

namespace AshborneGame._Core.Globals.Interfaces
{
    public interface ILocation
    {
        string Id { get; }
        string Name { get; }
        string ReferenceName { get; }
        string Description { get; }
        string AdditionalDescription { get; }
        string PositionalPrefix { get; }
        Dictionary<string, (Func<string> message, Action effect)> CustomCommands { get; }
        IReadOnlyDictionary<string, Location> Exits { get; }
        IReadOnlyList<Sublocation> Sublocations { get; }
        List<(Func<GameStateManager, bool> condition, string description)> Conditions { get; }
        int TimesVisited { get; set; }
        void AddExit(string direction, Location location);
        void AddSublocation(Sublocation sublocation);
        string GetExits(Player player);
        string GetSublocations(Player player);
        void AddCustomCommand(List<string> commands, Func<string> messageFunc, Action effect);
        bool CanPlayerSeeExit(Player player);
        void AddCondition(Func<GameStateManager, bool> condition, string description);
        string GetDescription(Player player, GameStateManager state);
        string GetFullDescription(Player player);
    }
}
