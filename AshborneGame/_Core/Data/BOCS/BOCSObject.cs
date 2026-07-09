using AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using System.Runtime.CompilerServices;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours;

namespace AshborneGame._Core.Data.BOCS;

/// <summary>
/// An abstract class representing any object in the game using the BOCS architecture.
/// Contains attributes and methods that allow for tracking and modification of Behaviours on this object.
/// </summary>
public class BOCSObject
{
    /// <summary>
    /// Gets the name of the object. This is used for identification and display.
    /// </summary>
    public ObjectNameAdapter Name { get; }

    public string Description { get; }

    public InstanceID InstanceID { get; init; }

    public DefinitionID DefinitionID { get; init; }

    /// <summary>
    /// Represents the Behaviours attached to this BOCSGameObject.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The key is a specific behaviour module type, and the value is a List containing all the behaviours that implement that module.
    /// </para>
    /// Note that a behaviour attached to this BOCSGameObject may implement multiple modules, and thus would be referenced in multiple key-value pairs.
    /// </remarks>
    public Dictionary<Type, List<Behaviour>> ByModule { get; private set; } = new();

    public List<Behaviour> ByBehaviour { get; private set; } = new();


    public BOCSObject(ObjectNameAdapter name, string description, DefinitionID definitionID, InstanceID instanceID = new())
    {
        DefinitionID = definitionID;
        InstanceID = instanceID;

        Name = name;
        Description = description;
    }

    #region Behaviours
    /// <summary>
    /// Adds a given Behaviour to this BOCSGameObject.
    /// </summary>
    /// <param name="type">The main module type the Behaviour implements. Used as the key in the key-value pair when adding this Behaviour to the Behaviours dictonary.</param>
    /// <param name="behaviour">The Behaviour to be added.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async void AddBehaviour(Type type, Behaviour behaviour)
    {
        // Throw if null
        if (type == null || behaviour == null)
            throw new ArgumentNullException();

        if (!type.IsAssignableFrom(behaviour.GetType()))
            throw new ArgumentException($"The provided behaviour does not implement or inherit from the specified type: {type.FullName}");

        // Enforce behaviour dependencies
        if (type == typeof(IActOnUse) && !ByModule.ContainsKey(typeof(IUsable)))
            throw new InvalidOperationException($"Cannot add IActOnUse without IUsable. {Name} must be usable before it can act on use.");

        if (type == typeof(IActOnEquip) && !ByModule.ContainsKey(typeof(IEquippable)))
            throw new InvalidOperationException($"Cannot add IActOnEquip without IEquippable. {Name} must be equippable before it can act on equip.");

        // Initialise the list if it doesn't exist
        if (!ByModule.ContainsKey(type))
        {
            ByModule[type] = new List<Behaviour>();
        }

        // Add the behavior to the list
        ByModule[type].Add(behaviour);
        ByBehaviour.Add(behaviour);

        if (type == typeof(IUsable))
        {
            await IOService.Output.DisplayDebugMessage($"{Name} is now usable.", ConsoleMessageTypes.ERROR);
        }

        behaviour.Owner = this;

#if DEBUG
        // Debug messages
        await IOService.Output.DisplayDebugMessage($"Added behaviour of type {type.FullName} to {Name}.", ConsoleMessageTypes.INFO);
        await IOService.Output.DisplayDebugMessage($"All registered behaviours for {Name}: {string.Join(", ", ByModule.Keys.Select(t => t.Name))}", ConsoleMessageTypes.INFO);
        foreach (var b in ByModule)
        {
            await IOService.Output.DisplayDebugMessage($"- {b.GetType().Name}: {string.Join(", ", b)}", ConsoleMessageTypes.INFO);
        }
#endif
    }

    /// <summary>
    /// Removes all Behaviours assigned to the key given.
    /// </summary>
    /// <remarks>
    /// Note that this does not fully remove any Behaviours who are registered with more than one module (key) in the Dictionary.
    /// </remarks>
    /// <typeparam name="T">The module type (key) that will be removed.</typeparam>
    public void RemoveBehaviour<T>() where T : class => ByModule.Remove(typeof(T));

    public void RemoveBehaviour(Behaviour behaviour)
    {
        if (behaviour == null) throw new ArgumentNullException(nameof(behaviour));
        // Remove from ByModule
        foreach (var key in ByModule.Keys.ToList())
        {
            ByModule[key].Remove(behaviour);
            if (ByModule[key].Count == 0)
            {
                ByModule.Remove(key);
            }
        }
        // Remove from ByBehaviour
        ByBehaviour.Remove(behaviour);
    }

    /// <summary>
    /// Tries to retrieve the first Behaviour registered in this BOCSGameObject that implements the given module.
    /// </summary>
    /// <typeparam name="T">The module type (key) that will be retrieved.</typeparam>
    /// <returns>
    /// An asynchronous Task where the first value is whether the retrieval was successful, and the second value is the Behaviour retrieved.
    /// If the operation was unsuccessful, T is null.
    /// </returns>
    public async Task<(bool, T?)> TryGetBehaviour<T>() where T : class
    {
#if DEBUG
        // Debug messages
        await IOService.Output.DisplayDebugMessage($"Attempting to get behaviour of type {typeof(T).FullName} from {Name}.", ConsoleMessageTypes.INFO);
        await IOService.Output.DisplayDebugMessage($"{Name} has Behaviours:", ConsoleMessageTypes.INFO);
        
        // Loop over each module and each Behaviour implementing that module and print it
        foreach (var kvp in ByModule)
        {
            foreach (var b in kvp.Value)
            {
                await IOService.Output.DisplayDebugMessage($"- {kvp.Key.Name}: {b.GetType().FullName}", ConsoleMessageTypes.INFO);
            }
        }
#endif
        // Check if the given module T exists in Behaviours
        if (ByModule.TryGetValue(typeof(T), out var behaviours) && behaviours.Count > 0 && behaviours[0] is T castedBehaviour)
        {
            // If it does, return the first Behaviour in the list
            await IOService.Output.DisplayDebugMessage($"Successfully retrieved behaviour of type {typeof(T).FullName} from {Name}", ConsoleMessageTypes.INFO);
            return (true, castedBehaviour);
        }
        // If not, return false and null
        await IOService.Output.DisplayDebugMessage($"Failed to retrieve behaviour of type {typeof(T).Name} from {Name}.", ConsoleMessageTypes.INFO);
        return (false, null);
    }

    public bool TryGetBehaviour<T>(out T behaviour) where T : Behaviour
    {
        if (ByBehaviour.FirstOrDefault() is T foundBehaviour)
        {
            behaviour = foundBehaviour;
            return true;
        }
        behaviour = null!;
        return false;
    }

    public bool HasBehaviours<T>() where T : class => ByModule.ContainsKey(typeof(T)) && ByModule[typeof(T)].Count > 0;

    public bool HasBehaviour<T>() where T : Behaviour => ByBehaviour.Any(b => b is T);

    /// <summary>
    /// Retrieves all Behaviours implementing the given module.
    /// </summary>
    /// <typeparam name="T">The module to retrieve.</typeparam>
    /// <returns>An IEnumerable containing a reference to all the Behaviours implementing the module T.</returns>
    public IEnumerable<T> GetAllBehavioursOfType<T>() where T : class
    {
        if (ByModule.TryGetValue(typeof(T), out var behaviours))
        {
            return behaviours.OfType<T>();
        }
        return Enumerable.Empty<T>();
    }

    #endregion Behaviours

    public bool IsItem() => HasBehaviours<StorableBehaviour>();

    // TODO: Make more exact
    public bool IsNPC() => HasBehaviours<TalkableBehaviour>();

    public bool IsObject() => !(IsItem() || IsNPC());
}
