using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.LocationManagement;

namespace AshborneGame._Core.Data.Definitions
{
    public static class LocationDefinitions
    {
        public static class Dreamspace
        {
            public static LocationDefinition EyePlatform = new(
                DefinitionIDs.Locations.Dreamspace.EyePlatform,
                new LocationNameAdapter("Eye Platform"),
                new DescriptionComposer(
                    new LookDescription(
                        "You glance around uneasily. The eye you stand on is unblinking and unmoving. Black clouds cover the sky, and the occasional lightning flashes are bright white against an otherwise dull and dark background.",
                        "You look around once more. Nothing changes — but are the shards sharper now?"),
                    new VisitDescription(
                        "You feel sick and disoriented. It takes you a few moments to stabilise. Glancing around, you notice that you're standing on an eye-shaped platform overlooking a vast, swirling abyss. The air is thick with an otherworldly energy as mirrors and shards of glass spin wildly around you.",
                        "You are back on the platform. The eye beneath seems stronger now, the pupil having enlarged, as though it wants to see more. The abyss feels darker, heavier.",
                        "For the fourth time, you stand overlooking the mess of glass and mirrors. You almost grow tired of it. The vortex is at its strongest now. The void is at its darkest, deepest, and the mirrors reflect your ragged face. It is unrecognisable now.",
                        "You are once again on the eye platform. It remains unchanged. The vortex belows continues swirling, and the eye continues staring."),
                    new SensoryDescription(
                        "The platform beneath is an alien stone, black and white patterns etched into every part of the eye.",
                        "It's eerily quiet despite the chaos above and below. As though the eye is remembering, and commanding everything to be silent."),
                    new AmbientDescription(new Dictionary<TimeSpan, string>() { { TimeSpan.FromSeconds(5), "The glass keeps on spinning around you. The eye does not blink." } }),
                    ConditionalDescription.StartNew()
                        // If the player has visited the Hall of Mirrors and this is their 1st, 2nd, or 4th visit to the Eye Platform
                        .If((player, gameState) =>
                        {
                            int hallOfMirrorsVisits = gameState.GetLocationVisitCount(new DefinitionID("locations.dreamspace.hall_of_mirrors"));
                            int currentVisits = player.CurrentLocation.VisitCount;
                            if (hallOfMirrorsVisits > 0 && 
                                (currentVisits == 1 || currentVisits == 2 || currentVisits == 4))
                            {
                                return true;
                            }
                            return false;
                        })
                        .ThenShow("And the mirrors reflect even deeper now, each questioning your very identity.")
                        .OnlyOnce(),
                    ConditionalDescription.StartNew()
                        // If the player has visited the Hall of Mirrors and this is their 3rd or later (>4) visit to the Eye Platform
                        .If((player, gameState) =>
                        {
                            int hallOfMirrorsVisits = gameState.GetLocationVisitCount(new DefinitionID("locations.dreamspace.hall_of_mirrors"));
                            int currentVisits = player.CurrentLocation.VisitCount;
                            if (hallOfMirrorsVisits > 0 &&
                                (currentVisits == 3 || currentVisits > 4))
                            {
                                return true;
                            }
                            return false;
                        })
                        .ThenShow("However, the mirrors seem to reflect even deeper into you now, each questioning your very identity.")
                        .OnlyOnce(),
                    ConditionalDescription.StartNew()
                        .If((player, gameState) =>
                        {
                            int templeVisits = gameState.GetLocationVisitCount(new DefinitionID("locations.dreamspace.temple_of_the_bound_one"));
                            bool talkedToBound = gameState.TryGetFlag(StateKeys.Flags.Player.Actions.In.OssanethsDomain.TalkedToBoundOne, out bool v2) && v2;
                            int visits = player.CurrentLocation.VisitCount;

                            if (templeVisits > 0 &&
                                talkedToBound &&
                                (visits == 1 || visits == 2 || visits == 4))
                            {
                                return true;
                            }
                            return false;
                        })
                        .ThenShow("However, now the swirl almost reminds you of the Bound One — chaotic, unnerving and unpredictable. You shiver. Maybe it's best not to think about him.")
                        .OnlyOnce(),
                    ConditionalDescription.StartNew()
                        .If((player, gameState) =>
                        {
                            int templeVisits = gameState.GetLocationVisitCount(new DefinitionID("locations.dreamspace.temple_of_the_bound_one"));
                            bool talkedToBound = gameState.TryGetFlag(StateKeys.Flags.Player.Actions.In.OssanethsDomain.TalkedToBoundOne, out bool v2) && v2;
                            int visits = player.CurrentLocation.VisitCount;

                            if (templeVisits > 0 && talkedToBound && (visits == 3 || visits > 4))
                            {
                                return true;
                            }
                            return false;
                        })
                ),
                [DefinitionIDs.Locations.Dreamspace.PlatformEdge],
                [],
                [],
                new()
            );
        }
    }
}
