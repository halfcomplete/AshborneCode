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

        // TODO: replace console.writeline with IOService
        // TODO: add support for single-word commands such as "back" or "enter"
        /// <summary>
        /// Tries to move the player to a valid location given the arguments.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="args">The list of words that DON'T include the command verb.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<bool> Move(Player player, List<string> args)
        {
            var current = player.CurrentLocation;
            var arguments = string.Join(" ", args).ToLowerInvariant();

            // 1. Parent ("leave", "back")
            if ((arguments == "back" || arguments == "leave") && current.Parent != null)
            {
                await player.MoveTo(current.Parent);
                return true;
            }

            // 2. Child locations
            var child = current.Children.FirstOrDefault(c =>
                c.Name.Matches(arguments));

            if (child != null)
            {
                await player.MoveTo(child);
                return true;
            }

            // 3. Explicit exits
            var exit = current.Exits.FirstOrDefault(e =>
                e.Direction.Equals(arguments,
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
