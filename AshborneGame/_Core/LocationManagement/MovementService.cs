using AshborneGame._Core._Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.LocationManagement
{
    public sealed class MovementService
    {
        private readonly ILocationRegistry _locationRegistry;

        public MovementService(ILocationRegistry locationRegistry)
        {
            _locationRegistry = locationRegistry;
        }

        // replace console.writeline with IOService
        public async Task<bool> Move(Player player, string direction)
        {
            var current = player.CurrentLocation;

            // 1. Parent ("leave", "back")
            if ((direction == "back" || direction == "leave") &&
                current.Parent != null)
            {
                await player.MoveTo(current.Parent);
                return true;
            }

            // 2. Child locations
            var child = current.Children.FirstOrDefault(c =>
                c.Name.Matches(direction));

            if (child != null)
            {
                await player.MoveTo(child);
                return true;
            }

            // 3. Explicit exits
            var exit = current.Exits.FirstOrDefault(e =>
                e.Direction.Equals(direction,
                    StringComparison.OrdinalIgnoreCase));

            if (exit != null)
            {
                if (exit.CanTraverse != null &&
                    !exit.CanTraverse())
                {
                    Console.WriteLine(exit.FailureMessage);
                    return false;
                }

                if (!_locationRegistry.TryGetLocationByDefinitionID(exit.TargetLocation, out var destination))
                {
                    return false;
                    throw new InvalidOperationException(
                        $"Unknown location '{exit.TargetLocation}'.");
                }

                await player.MoveTo(destination!);
                return true;
            }

            return false;
        }
    }
}
