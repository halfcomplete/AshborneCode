using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags;
using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Globals.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Game.Events
{
    public static class ExternalEventBuilder
    {
        public static string EventName { get; private set; } = "";
        public static Dictionary<string, List<MemoryRole>> EventParticipants { get; private set; } = new();
        public static Dictionary<string, string> EventData { get; private set; } = new();

        public static void BeginNew(string eventName)
        {
            EventName = eventName;
        }

        public static void AddParticipant(string participantId, List<MemoryRole> memoryRoles)
        {
            EventParticipants[participantId] = memoryRoles;
        }

        public static void AddData(string dataName, string dataValue)
        {
            EventData[dataName] = dataValue;
        }

        public static void Commit()
        {
            if (string.IsNullOrWhiteSpace(EventName))
            {
                throw new InvalidOperationException("[ExternalEventBuilder] Cannot commit event: EventName is empty.");
            }

            int currentTotalHours = GameContext.TimeTracker.TotalInGameHours;
            string kind = GetDataOrDefault("kind", "auto").Trim().ToLowerInvariant();

            if (kind == "synthetic" || EventName.StartsWith("synthetic.", StringComparison.OrdinalIgnoreCase))
            {
                CommitSynthetic(currentTotalHours);
                return;
            }

            if (kind == "memorable" || EventData.ContainsKey("memoryTags") || EventData.ContainsKey("baseIntensity"))
            {
                CommitMemorable(currentTotalHours);
                return;
            }

            if (ExternalGameEventFactory.TryCreateNormalEvent(EventName, currentTotalHours, EventData, out IGameEvent? gameEvent) && gameEvent != null)
            {
                PublishConcreteEvent(gameEvent);
                return;
            }

            throw new InvalidOperationException($"[ExternalEventBuilder] Unknown event '{EventName}'. Add a mapping in ExternalGameEventFactory or mark it as memorable/synthetic via event data key 'kind'.");
        }


        private static void CommitMemorable(int currentTotalHours)
        {
            MemoryDefinition memoryDefinition = BuildMemoryDefinitionFromData();
            List<MemoryParticipant> participants = BuildParticipants();

            if (participants.Count == 0)
            {
                participants.Add(new MemoryParticipant(Guid.Empty, [MemoryRole.Actor]));
            }

            Guid locationID = TryParseGuid(GetDataOrDefault("locationGuid", "")) ?? Guid.Empty;

            IMemorableGameEvent memorableEvent = ExternalGameEventFactory.CreateExternalMemorableEvent(
                EventName,
                currentTotalHours,
                memoryDefinition,
                participants,
                locationID,
                EventData);

            // Memory profiles currently subscribe to IMemorableGameEvent, so we publish as that interface type.
            EventBus.Publish<IMemorableGameEvent>(memorableEvent);
        }

        private static void CommitSynthetic(int currentTotalHours)
        {
            MemoryDefinition memoryDefinition = BuildMemoryDefinitionFromData();
            List<MemoryParticipant> participants = BuildParticipants();

            string sourceLabel = GetDataOrDefault("sourceLabel", EventName);
            Guid locationID = TryParseGuid(GetDataOrDefault("locationGuid", "")) ?? Guid.Empty;

            List<(ISentientEntity Entity, Guid EntityId)> targets = ResolveSyntheticTargets(participants);

            foreach ((ISentientEntity entity, Guid entityId) in targets)
            {
                List<MemoryParticipant> targetParticipants = CloneParticipants(participants);
                if (targetParticipants.All(p => p.EntityId != entityId))
                {
                    targetParticipants.Add(new MemoryParticipant(entityId, [MemoryRole.Actor]));
                }

                entity.PsychologicalState.MemoryProfile.ReceiveSyntheticMemory(
                    sourceLabel,
                    memoryDefinition,
                    currentTotalHours,
                    locationID,
                    targetParticipants);
            }
        }

        private static List<MemoryParticipant> BuildParticipants()
        {
            List<MemoryParticipant> participants = new();

            foreach ((string participantIdText, List<MemoryRole> roles) in EventParticipants)
            {
                Guid? parsedId = ParseEntityId(participantIdText);
                if (parsedId == null)
                {
                    continue;
                }

                List<MemoryRole> parsedRoles = roles.Count > 0 ? roles : [MemoryRole.Witness];
                participants.Add(new MemoryParticipant(parsedId.Value, parsedRoles));
            }

            return participants;
        }

        private static List<MemoryParticipant> CloneParticipants(List<MemoryParticipant> participants)
        {
            List<MemoryParticipant> clone = new();

            foreach (MemoryParticipant participant in participants)
            {
                clone.Add(new MemoryParticipant(participant.EntityId, [.. participant.Roles]));
            }

            return clone;
        }

        private static MemoryDefinition BuildMemoryDefinitionFromData()
        {
            string tagsCsv = GetDataOrDefault("memoryTags", GetDataOrDefault("tags", ""));

            HashSet<MemoryTag> tags = ParseMemoryTags(tagsCsv);

            if (tags.Count == 0)
            {
                tags = [MemoryTag.Theft];
            }

            double baseIntensity = TryParseDouble(GetDataOrDefault("baseIntensity", "0.4")) ?? 0.4;
            baseIntensity = Math.Clamp(baseIntensity, 0.0, 1.0);

            return new MemoryDefinition(baseIntensity, tags);
        }

        private static HashSet<MemoryTag> ParseMemoryTags(string tagsCsv)
        {
            HashSet<MemoryTag> tags = new();

            if (string.IsNullOrWhiteSpace(tagsCsv))
            {
                return tags;
            }

            foreach (string tagText in tagsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (Enum.TryParse<MemoryTag>(tagText, true, out MemoryTag tag) && MemoryTagDefinitions.Definitions.ContainsKey(tag))
                {
                    tags.Add(tag);
                }
            }

            return tags;
        }

        private static List<(ISentientEntity Entity, Guid EntityId)> ResolveSyntheticTargets(List<MemoryParticipant> participants)
        {
            List<(ISentientEntity Entity, Guid EntityId)> targets = new();

            Guid? targetEntityId = ParseEntityId(GetDataOrDefault("target", GetDataOrDefault("targetId", "")));
            if (targetEntityId != null)
            {
                ISentientEntity? targetEntity = ResolveEntityById(targetEntityId.Value);
                if (targetEntity != null)
                {
                    targets.Add((targetEntity, targetEntityId.Value));
                }
            }

            if (targets.Count == 0)
            {
                foreach (MemoryParticipant participant in participants)
                {
                    ISentientEntity? entity = ResolveEntityById(participant.EntityId);
                    if (entity != null && targets.All(t => t.EntityId != participant.EntityId))
                    {
                        targets.Add((entity, participant.EntityId));
                    }
                }
            }

            if (targets.Count == 0)
            {
                // Player currently has Guid.Empty in this codebase.
                targets.Add((GameContext.Player, Guid.Empty));
            }

            return targets;
        }

        private static ISentientEntity? ResolveEntityById(Guid entityId)
        {
            if (entityId == Guid.Empty)
            {
                return GameContext.Player;
            }

            if (GameContext.Player.CurrentScene?.Locations == null)
            {
                return null;
            }

            foreach (var location in GameContext.Player.CurrentScene.Locations)
            {
                foreach (var sublocation in location.Sublocations)
                {
                    if (sublocation.FocusObject is ISentientEntity sentient && sublocation.FocusObject.ID == entityId)
                    {
                        return sentient;
                    }
                }
            }

            return null;
        }

        private static void PublishConcreteEvent(IGameEvent gameEvent)
        {
            var publishMethod = typeof(EventBus)
                .GetMethods()
                .First(m => m.Name == nameof(EventBus.Publish) && m.IsGenericMethodDefinition && m.GetParameters().Length == 1)
                .MakeGenericMethod(gameEvent.GetType());

            publishMethod.Invoke(null, [gameEvent]);
        }

        private static Guid? ParseEntityId(string rawId)
        {
            if (string.IsNullOrWhiteSpace(rawId))
            {
                return null;
            }

            string trimmed = rawId.Trim();

            if (trimmed.Equals("player", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("self", StringComparison.OrdinalIgnoreCase))
            {
                return Guid.Empty;
            }

            return TryParseGuid(trimmed);
        }

        private static Guid? TryParseGuid(string rawGuid)
        {
            return Guid.TryParse(rawGuid, out Guid parsedGuid) ? parsedGuid : null;
        }

        private static double? TryParseDouble(string rawDouble)
        {
            return double.TryParse(rawDouble, out double parsedDouble) ? parsedDouble : null;
        }

        private static string GetDataOrDefault(string key, string fallback)
        {
            return EventData.TryGetValue(key, out string? value) ? value : fallback;
        }

        public static void Clear()
        {
            EventName = "";
            EventParticipants = new();
            EventData = new();
        }
    }
}
