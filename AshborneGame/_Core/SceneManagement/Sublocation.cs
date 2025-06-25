using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Interfaces;

namespace AshborneGame._Core.SceneManagement
{
    /// <summary>
    /// Represents a sublocation within a location that contains an interactive object.
    /// </summary>
    public class Sublocation : Location
    {
        /// <summary>
        /// Gets the parent location of this sublocation.
        /// </summary>
        public Location ParentLocation { get; }

        /// <summary>
        /// Gets the game object associated with this sublocation.
        /// </summary>
        public BOCSGameObject Object { get; }

        /// <summary>
        /// Initializes a new instance of the Subscene class.
        /// </summary>
        /// <param name="parentLocation">The parent location of this sublocation.</param>
        /// <param name="object">The game object associated with this sublocation.</param>
        /// <param name="name">The name of the sublocation.</param>
        /// <param name="description">The description of the sublocation.</param>
        /// <param name="minimumVisibility">The minimum visibility level required to see this sublocation.</param>
        /// <exception cref="ArgumentNullException">Thrown when parentLocation, object, name, or description is null.</exception>
        public Sublocation(Location parentLocation, BOCSGameObject @object, string name, string description, int minimumVisibility)
            : base(name, description, minimumVisibility)
        {
            ParentLocation = parentLocation;
            Object = @object;
        }

        /// <summary>
        /// Determines if the player can see this sublocation.
        /// </summary>
        /// <returns>True if the player can see this sublocation; otherwise, false.</returns>
        public bool CanPlayerSeeSublocation(Player player) => CanPlayerSeeExit(player);

        public override string GetDescription(Player player, GameStateManager state)
        {
            string contextualDescription = Description;
            if (player.EquippedItems.Any(s => s.Value != null && s.Value.Name.Equals("torch", StringComparison.OrdinalIgnoreCase)))
            {
                contextualDescription += $". It is barely lit by your torch.";
            }

            string additionalDescription = string.Empty;
            if (Object.TryGetBehaviour<IDescribable>(out var describable))
                additionalDescription = describable.GetDescription(player, state);
            return $"You are at {Name}. {contextualDescription} {additionalDescription}";
        }
    }
}
