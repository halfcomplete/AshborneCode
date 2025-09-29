using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using System.Runtime.CompilerServices;

namespace AshborneGame._Core.Data.BOCS;

public abstract class BOCSGameObject
{
    /// <summary>
    /// Gets the name of the object. This is used for identification and display.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the unique identifier of the object.
    /// </summary>
    public string ID { get; init; } = Guid.NewGuid().ToString();

    public Dictionary<Type, List<object>> Behaviours { get; private set; } = new();

    #region Behaviours
    public async void AddBehaviour(Type type, object behaviour)
    {
        if (type == null || behaviour == null)
            throw new ArgumentNullException();

        if (!type.IsAssignableFrom(behaviour.GetType()))
            throw new ArgumentException($"The provided behaviour does not implement or inherit from the specified type: {type.FullName}");

        // Enforce behaviour dependencies
        if (type == typeof(IActOnUse) && !Behaviours.ContainsKey(typeof(IUsable)))
            throw new InvalidOperationException($"Cannot add IActOnUse without IUsable. {Name} must be usable before it can act on use.");

        if (type == typeof(IActOnEquip) && !Behaviours.ContainsKey(typeof(IEquippable)))
            throw new InvalidOperationException($"Cannot add IActOnEquip without IEquippable. {Name} must be equippable before it can act on equip.");

        // Initialize the list if it doesn't exist
        if (!Behaviours.ContainsKey(type))
        {
            Behaviours[type] = new List<object>();
        }

        // Add the behavior to the list
        Behaviours[type].Add(behaviour);

        if (type == typeof(IUsable))
        {
            await IOService.Output.DisplayDebugMessage($"{Name} is now usable.", ConsoleMessageTypes.ERROR);
        }

        await IOService.Output.DisplayDebugMessage($"Added behaviour of type {type.FullName} to {Name}.", ConsoleMessageTypes.INFO);
        await IOService.Output.DisplayDebugMessage($"All registered behaviours for {Name}: {string.Join(", ", Behaviours.Keys.Select(t => t.Name))}", ConsoleMessageTypes.INFO);
        foreach (var b in Behaviours)
        {
            await IOService.Output.DisplayDebugMessage($"- {b.GetType().Name}: {string.Join(", ", b)}", ConsoleMessageTypes.INFO);
        }
    }

    public void RemoveBehaviour<T>() where T : class => Behaviours.Remove(typeof(T));

    public async Task<(bool, T)> TryGetBehaviour<T>() where T : class
    {
        await IOService.Output.DisplayDebugMessage($"Attempting to get behaviour of type {typeof(T).FullName} from {Name}.", ConsoleMessageTypes.INFO);
        await IOService.Output.DisplayDebugMessage($"{Name} has Behaviours:", ConsoleMessageTypes.INFO);
        foreach (var kvp in Behaviours)
        {
            foreach (var b in kvp.Value)
            {
                await IOService.Output.DisplayDebugMessage($"- {kvp.Key.Name}: {b.GetType().FullName}", ConsoleMessageTypes.INFO);
            }
        }
        if (Behaviours.TryGetValue(typeof(T), out var behaviours) && behaviours.Count > 0 && behaviours[0] is T castedBehaviour)
        {
            await IOService.Output.DisplayDebugMessage($"Successfully retrieved behaviour of type {typeof(T).FullName} from {Name}", ConsoleMessageTypes.INFO);
            return (true, castedBehaviour);
        }
        await IOService.Output.DisplayDebugMessage($"Failed to retrieve behaviour of type {typeof(T).Name} from {Name}.", ConsoleMessageTypes.INFO);
        return (false, null!);
    }

    public bool HasBehaviours<T>() where T : class => Behaviours.ContainsKey(typeof(T)) && Behaviours[typeof(T)].Count > 0;

    public IEnumerable<T> GetAllBehaviours<T>() where T : class
    {
        if (Behaviours.TryGetValue(typeof(T), out var behaviours))
        {
            return behaviours.OfType<T>();
        }
        return Enumerable.Empty<T>();
    }

    #endregion Behaviours
}
