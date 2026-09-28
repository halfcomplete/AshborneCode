# Memory Tag Definition Guide

This guide describes how to add entries to `MemoryTagDefinitions.Definitions`. Each registry entry maps a `MemoryTagType` to an `IMemoryTag`, whose `Definition` contains the rules applied when a memory has that tag.

## Registry Shape

`MemoryTagDefinitions.Definitions` is a `Dictionary<MemoryTagType, IMemoryTag>`. Every tag used in a `MemoryDefinition.Tags` set must have a matching registry entry, or memory creation will fail when it looks up the definition.

`IMemoryTag` requires two properties:

- `Type`: the matching `MemoryTagType` enum value.
- `Definition`: the `MemoryTagDefinition` containing the effects for this tag.

There is no concrete `IMemoryTag` implementation in the current code, so a small record can provide the wrapper:

```csharp
public sealed record MemoryTag(MemoryTagType Type, MemoryTagDefinition Definition) : IMemoryTag;
```

Add the corresponding enum member in `MemoryTagType` before using a new tag in memories.

## The Six Definition Inputs

`MemoryTagDefinition` takes these six dictionaries, in this order. Empty dictionaries are valid when a tag has no rules of a particular kind.

| Constructor input / resulting property | Dictionary structure | Purpose |
| --- | --- | --- |
| `initialEmotions` / `InitialEmotions` | Subject role -> entries of `(optional target role, emotion -> value)` | Adds baseline emotion potentials for the role in the memory. A null target role makes the emotion non-directed; a target role associates it with participants in that role. |
| `intensityRules` / `IntensityRules` | Subject role -> list of values | Adds base memory intensity when the NPC has that role. |
| `relationshipEmotionRules` / `RelationshipEmotionRules` | Subject role -> target role -> relationship type -> `(emotion -> value)` | Changes or adds an emotion response when the subject has the specified relationship toward someone in the target role. |
| `attitudeIntensityRules` / `RelationshipIntensityRules` | Subject role -> target role -> relationship type -> value | Changes memory intensity according to the subject's relationship toward participants in the target role. |
| `personalityEmotionRules` / `PersonalityEmotionRules` | Personality trait -> subject role -> `(emotion -> value)` | Scales or adds an emotion response according to the subject's personality trait. |
| `personalityIntensityRules` / `PersonalityIntensityRules` | Personality trait -> subject role -> value | Adds a personality-scaled change to memory intensity for the subject role. |

The constructor flattens these nested dictionaries into lists of `EmotionPotential`, `IntensityRule`, `RelationshipEmotionRule`, `RelationshipIntensityRule`, `PersonalityEmotionRule`, and `PersonalityIntensityRule`. The dictionary keys identify the subject or trait; the nested tuple fields identify targets, relationships, and individual emotion values.

## Authoring Example

This complete example gives `Theft` baseline effects, relationship responses, and personality reactions. Remove or leave empty any category that does not apply.

The example needs these namespaces in scope:

```csharp
using System.Collections.Generic;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;
using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags;
```

```csharp
var initialEmotions = new Dictionary<MemoryRole, List<(MemoryRole? target, List<(EmotionType emotion, double value)> emotionValues)>>
{
    [MemoryRole.Target] = new()
    {
        (MemoryRole.Actor, new() { (EmotionType.Anger, 0.6), (EmotionType.Sadness, 0.3) })
    }
};

var intensityRules = new Dictionary<MemoryRole, List<double>>
{
    [MemoryRole.Target] = new() { 0.5 },
    [MemoryRole.Actor] = new() { 0.2 }
};

var relationshipEmotionRules = new Dictionary<MemoryRole, List<(MemoryRole target, List<(RelationshipType relationship, List<(EmotionType emotion, double value)> emotionValues)> relationships)>>
{
    [MemoryRole.Target] = new()
    {
        (MemoryRole.Actor, new()
        {
            (RelationshipType.Loves, new() { (EmotionType.Sadness, 0.5) }),
            (RelationshipType.Hates, new() { (EmotionType.Anger, 1.4) })
        })
    }
};

var attitudeIntensityRules = new Dictionary<MemoryRole, List<(MemoryRole target, List<(RelationshipType relationship, double value)> relationships)>>
{
    [MemoryRole.Target] = new()
    {
        (MemoryRole.Actor, new() { (RelationshipType.Loves, 1.2), (RelationshipType.Hates, 0.8) })
    }
};

var personalityEmotionRules = new Dictionary<PersonalityTrait, List<(MemoryRole self, List<(EmotionType emotion, double value)> emotionValues)>>
{
    [PersonalityTrait.Aggression] = new()
    {
        (MemoryRole.Target, new() { (EmotionType.Anger, 1.5) })
    }
};

var personalityIntensityRules = new Dictionary<PersonalityTrait, List<(MemoryRole self, double value)>>
{
    [PersonalityTrait.Compassion] = new() { (MemoryRole.Target, 1.2) }
};

var theftTag = new MemoryTag(
    MemoryTagType.Theft,
    new MemoryTagDefinition(
        initialEmotions,
        intensityRules,
        relationshipEmotionRules,
        attitudeIntensityRules,
        personalityEmotionRules,
        personalityIntensityRules));

```

## Enums

```csharp
public enum EmotionType
{
    Anger,
    Contempt,
    Disgust,
    Fear,
    Happiness,
    Sadness,
    Surprise
}

public enum AttitudeFactor
{
    Affection,
    Respect,
    Trust,
    Fear,
    Dominance
}

public enum RelationshipType
{
    Loves,
    Hates,
    Trusts,
    Distrusts,
    Respects,
    Disrespects,
    Fears,
    DoesNotFear,
    Dominates,
    Submits,
}

public enum MemoryRole
{
    Actor,
    Target,
    Witness
}
```

## Value Semantics

All effects use `double`; the constructor does not validate or clamp the rule values.

- Initial emotion values seed a role's emotion potential. Positive and negative values represent increasing and decreasing influence, respectively.
- Intensity rules use multiplicative values from `0.0` to `2.0`, where `1.0` is neutral, `0.0` suppresses the contribution, and `2.0` doubles it. The multiplier is interpolated from neutral using the personality trait or relationship alignment. The final intensity is clamped to `0.0..1.0` after contributions are combined.
- Personality trait values in `PersonalityProfile` are intended to be between `0.0` and `1.0`. An emotion rule's value acts as the response multiplier at full trait strength: `1.0` leaves the response unchanged, above `1.0` increases it, and between `0.0` and `1.0` reduces it.
- Relationship rules select the relationship direction (`Loves` vs `Hates`, `Trusts` vs `Distrusts`, and so on). The engine uses the magnitude of the matching attitude alignment. Relationship emotion values modify a matching emotion response; relationship intensity values are neutral-centered multipliers from `0.0` to `2.0`.
- Emotion modifiers (both personality and relationship) are multiplicative and are added to the TotalMult using the formula:
```csharp
currentTotalMult = currentTotalMult * (1 + personalityTraitInfluence * (reactionMult - 1));
```
- Intensity modifiers are multiplicative and must be in the range `0.0..2.0`.

Rules only apply when the memory participant has the configured subject role. Relationship rules additionally require a participant matching the target role and an attitude entry for that participant. Personality rules require that the subject has the configured role and that the personality profile contains the trait.

## Authoring Checklist

1. Add a `MemoryTagType` enum member if this is a new tag.
2. Create one `MemoryTagDefinition`, passing the six dictionaries in constructor order. Use an empty dictionary for categories with no effects.
3. Wrap it in an `IMemoryTag` implementation whose `Type` matches the registry key.
4. Add the entry to `MemoryTagDefinitions.Definitions` before the tag can appear in a `MemoryDefinition`.
5. Check each rule's subject role, target role, relationship direction, multiplier range, and test the resulting emotion and intensity behavior in a memory.