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
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.LocationManagement;
using AshborneGame._Core.Data.Definitions;

namespace AshborneGame._Core.Game.Events
{
    public static class ExternalEventBuilder
    {
        public static string EventName { get; private set; } = "";
        public static Dictionary<DefinitionID, List<MemoryRole>> EventParticipants { get; private set; } = new();
        public static Dictionary<string, string> EventData { get; private set; } = new();

        private static IDefinitionRegistry? _definitionRegistry;
        private static IInstanceRegistry? _instanceRegistry;
        private static ILocationRegistry? _locationRegistry;

        public static void Initialise(IDefinitionRegistry definitionRegistry, IInstanceRegistry instanceRegistry, ILocationRegistry locationRegistry)
        {
            _definitionRegistry = definitionRegistry;
            _instanceRegistry = instanceRegistry;
            _locationRegistry = locationRegistry;
        }

        public static void BeginNew(string eventName)
        {
            EventName = eventName;
        }

        public static void AddParticipant(string participantId, List<MemoryRole> memoryRoles)
        {
            EventParticipants[new DefinitionID(participantId)] = memoryRoles;
        }

        public static void AddData(string dataName, string dataValue)
        {
            EventData[dataName] = dataValue;
        }

        public static void Commit(ILocationRegistry locationRegistry, IDefinitionRegistry definitionRegistry)
        {
            if (string.IsNullOrWhiteSpace(EventName))
            {
                throw new InvalidOperationException("[ExternalEventBuilder] Cannot commit event: EventName is empty.");
            }

            int currentTotalHours = GameContext.TimeTracker.TotalInGameHours;
            string type = GetDataOrDefault("type", "auto").Trim().ToLowerInvariant();

            if (type == "synthetic" || EventName.StartsWith("synthetic.", StringComparison.OrdinalIgnoreCase))
            {
                CommitSynthetic(currentTotalHours, definitionRegistry);
                return;
            }

            if (type == "memorable" || EventData.ContainsKey("memoryTags") || EventData.ContainsKey("baseIntensity"))
            {
                CommitMemorable(currentTotalHours, locationRegistry, definitionRegistry);
                return;
            }

            if (ExternalGameEventFactory.TryCreateNormalEvent(EventName, currentTotalHours, EventData, out IGameEvent? gameEvent) && gameEvent != null)
            {
                PublishConcreteEvent(gameEvent);
                return;
            }

            throw new InvalidOperationException($"[ExternalEventBuilder] Unknown event '{EventName}'. Add a mapping in ExternalGameEventFactory or mark it as memorable/synthetic via event data key 'type'.");
        }

        private static void CommitMemorable(int currentTotalHours, ILocationRegistry locationRegistry, IDefinitionRegistry definitionRegistry)
        {
            MemoryDefinition memoryDefinition = BuildMemoryDefinitionFromData();
            List<MemoryParticipant> participants = BuildParticipants(definitionRegistry);

            DefinitionID locationID = new(GetDataOrDefault("locationID", ""));

            if (locationID.Value == "")
            {
                throw new ArgumentException($"[ExternalEventBuilder] Cannot commit memorable event '{EventName}': locationID is required in EventData.");
            }

            if (!locationRegistry.TryGetLocationByDefinitionID(locationID, out _))
            {
                throw new ArgumentException($"[ExternalEventBuilder] Cannot commit memorable event '{EventName}': locationID {locationID.Value} in EventData is not valid.");
            }

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

        private static void CommitSynthetic(int currentTotalHours, IDefinitionRegistry definitionRegistry)
        {
            MemoryDefinition memoryDefinition = BuildMemoryDefinitionFromData();
            List<MemoryParticipant> participants = BuildParticipants(definitionRegistry);

            DefinitionID locationID = new(GetDataOrDefault("locationID", ""));

            if (locationID.Value == "")
            {
                throw new ArgumentException($"[ExternalEventBuilder] Cannot commit synthetic event '{EventName}': locationID is required in EventData.");
            }

            List<(ISentientEntity Entity, DefinitionID EntityId)> targets = ResolveSyntheticTargets(participants);

            foreach ((ISentientEntity entity, DefinitionID entityId) in targets)
            {
                List<MemoryParticipant> targetParticipants = CloneParticipants(participants);
                if (targetParticipants.All(p => p.EntityId != entityId))
                {
                    targetParticipants.Add(new MemoryParticipant(entityId, [MemoryRole.Actor]));
                }

                entity.PsychologicalState.Memory.ReceiveSyntheticMemory(
                    memoryDefinition,
                    currentTotalHours,
                    locationID,
                    targetParticipants);
            }
        }

        /// <summary>
        /// Builds a list of MemoryParticipant objects based on the EventParticipants dictionary.
        /// </summary>
        /// <returns></returns>
        private static List<MemoryParticipant> BuildParticipants(IDefinitionRegistry definitionRegistry)
        {
            List<MemoryParticipant> participants = new();

            foreach ((DefinitionID definitionID, List<MemoryRole> roles) in EventParticipants)
            {
                if (!definitionRegistry.TryGet<Definition>(definitionID, out var _))
                {
                    throw new InvalidOperationException($"Building Participants: DefinitionID {definitionID} doesn't exist.");
                }
                List<MemoryRole> parsedRoles = roles.Count > 0 ? roles : [MemoryRole.Witness];
                participants.Add(new MemoryParticipant(definitionID, parsedRoles));
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

        /// <summary>
        /// Builds a MemoryDefinition object based on the EventData dictionary, specifically looking for "memoryTags" and "baseIntensity" keys. If no tags are provided, it defaults to a Theft tag. The base intensity is clamped between 0.0 and 1.0.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Parses a comma-separated string of memory tags into a HashSet of MemoryTag enums. If the input string is null or whitespace, an empty HashSet is returned. Only valid MemoryTag values are added to the HashSet.
        /// </summary>
        /// <param name="tagsCsv"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Resolves the target entities for a synthetic event based on the provided participants and event data. 
        /// If a specific target is defined in the event data, it will be used; otherwise, all participants will 
        /// be considered as targets. If no targets are found, the player entity will be used as a fallback.
        /// </summary>
        /// <param name="participants"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static List<(ISentientEntity Entity, DefinitionID EntityId)> ResolveSyntheticTargets(List<MemoryParticipant> participants)
        {
            List<(ISentientEntity Entity, DefinitionID EntityId)> targets = new();

            DefinitionID targetEntityId = new(GetDataOrDefault("target", GetDataOrDefault("targetID", "")));
            if (targetEntityId.Value != "")
            {
                ISentientEntity? targetEntity = ResolveEntityById(targetEntityId);
                if (targetEntity != null)
                {
                    targets.Add((targetEntity, targetEntityId));
                }
            }
            else
            {
                throw new InvalidOperationException($"Resolving Synthetic Targets: event data 'targetID' does not exist.");
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
                targets.Add((GameContext.Player, new DefinitionID("Player")));
            }

            return targets;
        }

        private static ISentientEntity? ResolveEntityById(DefinitionID entityId)
        {
            ConfirmInitialised();

            if (entityId.Value == "Player")
            {
                return GameContext.Player;
            }

            var targets = _instanceRegistry.GetByDefinition(entityId).ToList();

            if (targets.Count() != 1)
            {
                throw new InvalidOperationException($"Resolve Entity by ID: Number of instances with definition ID {entityId} was {targets.Count()}. Expected 1.");
            }

            var target = targets[0];

            if (target.HasBehaviours<ISentientEntity>())
            {
                return target.TryGetBehaviour<ISentientEntity>().Result.Item2;
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

        private static double? TryParseDouble(string rawDouble)
        {
            return double.TryParse(rawDouble, out double parsedDouble) ? parsedDouble : null;
        }

        /// <summary>
        /// Returns the value associated with the specified key from EventData, or the provided fallback value if the key does not exist.
        /// </summary>
        /// <param name="key">The key to look up in the EventData dictionary.</param>
        /// <param name="fallback">The value to return if the key does not exist in EventData.</param>
        /// <returns>The value associated with the specified key, or the fallback value if the key does not exist.</returns>
        private static string GetDataOrDefault(string key, string fallback)
        {
            return EventData.TryGetValue(key, out string? value) ? value : fallback;
        }

        private static void ConfirmInitialised()
        {
            ArgumentNullException.ThrowIfNull(_definitionRegistry);
            ArgumentNullException.ThrowIfNull(_instanceRegistry);
            ArgumentNullException.ThrowIfNull(_locationRegistry);
        }

        public static void Clear()
        {
            EventName = "";
            EventParticipants = new();
            EventData = new();
        }
    }
}
