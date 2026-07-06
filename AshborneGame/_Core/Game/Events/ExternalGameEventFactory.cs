using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.Data.IDSystem;

namespace AshborneGame._Core.Game.Events
{
    internal static class ExternalGameEventFactory
    {
        public static bool TryCreateNormalEvent(string eventName, int currentTotalHours, IReadOnlyDictionary<string, string> data, out IGameEvent? gameEvent)
        {
            string normalizedName = eventName.Trim().ToLowerInvariant();

            gameEvent = normalizedName switch
            {
                "player.prayed" => new GameEvents.Player.PrayedEvent(currentTotalHours, GetRequiredString(data, "locationGroup", "location")),
                "player.moved" => new GameEvents.Player.MovedEvent(currentTotalHours, GetOptionalString(data, "fromLocation", "from"), GetRequiredString(data, "toLocation", "to")),
                "player.itempickedup" => new GameEvents.Player.ItemPickedUpEvent(currentTotalHours, GetRequiredString(data, "itemName", "item"), GetRequiredString(data, "itemId")),
                "player.maskequipped" => new GameEvents.Player.MaskEquippedEvent(currentTotalHours, GetRequiredString(data, "maskName", "mask")),

                "dialogue.started" => new GameEvents.Dialogue.StartedEvent(currentTotalHours, GetRequiredString(data, "dialogueName", "dialogue")),
                "dialogue.ended" => new GameEvents.Dialogue.EndedEvent(currentTotalHours, GetRequiredString(data, "dialogueName", "dialogue")),
                "dialogue.choicemade" => new GameEvents.Dialogue.ChoiceMadeEvent(currentTotalHours, GetRequiredString(data, "dialogueName", "dialogue"), GetRequiredInt(data, "choiceIndex", "choice"), GetRequiredString(data, "choiceText", "text")),

                "quest.started" => new GameEvents.Quest.StartedEvent(currentTotalHours, GetRequiredString(data, "questId", "id"), GetRequiredString(data, "questName", "name")),
                "quest.completed" => new GameEvents.Quest.CompletedEvent(currentTotalHours, GetRequiredString(data, "questId", "id"), GetRequiredString(data, "questName", "name")),
                "quest.failed" => new GameEvents.Quest.FailedEvent(currentTotalHours, GetRequiredString(data, "questId", "id"), GetRequiredString(data, "questName", "name"), GetOptionalString(data, "reason")),
                "quest.objectiveupdated" => new GameEvents.Quest.ObjectiveUpdatedEvent(currentTotalHours, GetRequiredString(data, "questId", "id"), GetRequiredString(data, "objectiveId", "objective"), GetRequiredInt(data, "progress"), GetRequiredInt(data, "target")),

                "system.gamesaved" => new GameEvents.System.GameSavedEvent(currentTotalHours, GetRequiredString(data, "slotName", "slot")),
                "system.gameloaded" => new GameEvents.System.GameLoadedEvent(currentTotalHours, GetRequiredString(data, "slotName", "slot")),

                "ossanethsdomain.outrotriggered" => new GameEvents.OssanethsDomain.OutroTriggeredEvent(currentTotalHours),
                "ossanethsdomain.eyeplatformvisitthreshold" => new GameEvents.OssanethsDomain.EyePlatformVisitThresholdEvent(currentTotalHours, GetRequiredString(data, "locationName", "location"), GetRequiredInt(data, "visitCount", "count")),

                _ => null
            };

            return gameEvent != null;
        }

        public static IMemorableGameEvent CreateExternalMemorableEvent(
            string eventName,
            int currentTotalHours,
            MemoryDefinition memoryDefinition,
            List<MemoryParticipant> participants,
            DefinitionID locationID,
            IReadOnlyDictionary<string, string> eventData)
        {
            return new ExternalMemorableGameEvent(eventName, currentTotalHours, memoryDefinition, participants, locationID, new Dictionary<string, string>(eventData));
        }

        private static string GetRequiredString(IReadOnlyDictionary<string, string> data, params string[] keys)
        {
            foreach (string key in keys)
            {
                if (data.TryGetValue(key, out string? value) && !string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            throw new InvalidOperationException($"[ExternalGameEventFactory] Missing required event data key. Expected one of: {string.Join(", ", keys)}");
        }

        private static string? GetOptionalString(IReadOnlyDictionary<string, string> data, params string[] keys)
        {
            foreach (string key in keys)
            {
                if (data.TryGetValue(key, out string? value) && !string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return null;
        }

        private static int GetRequiredInt(IReadOnlyDictionary<string, string> data, params string[] keys)
        {
            string rawValue = GetRequiredString(data, keys);
            if (!int.TryParse(rawValue, out int parsedValue))
            {
                throw new InvalidOperationException($"[ExternalGameEventFactory] Expected integer for key '{keys[0]}', got '{rawValue}'.");
            }

            return parsedValue;
        }

        private sealed record ExternalMemorableGameEvent(
            string Name,
            int CurrentTotalHours,
            MemoryDefinition MemoryDefinition,
            List<MemoryParticipant> Participants,
            DefinitionID LocationID,
            Dictionary<string, string> Data) : IMemorableGameEvent;
    }
}
