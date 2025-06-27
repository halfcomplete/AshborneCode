using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours;
public class OnTurnStartAttackPlayerComponent : ICanAttackPlayer
{
    public Item Weapon { get; set; }
    public int BaseAttackPower { get; set; }

    public int AttackPower => BaseAttackPower + GetWeaponAttackPower();
    public int AttackDamage => AttackPower;

    public OnTurnStartAttackPlayerComponent(Item weapon, int baseAttackPower)
    {
        Weapon = weapon ?? throw new ArgumentNullException(nameof(weapon), "Weapon cannot be null.");
        BaseAttackPower = baseAttackPower;
    }

    private int GetWeaponAttackPower()
    {
        if (Weapon.TryGetBehaviour<ICanDamage>(out var component))
        {
            return component.BaseDamage;
        }
        return 0;
    }

    public void AttackPlayer(Player player)
    {
        player.ChangeHealth(-AttackDamage);
        IOService.Output.WriteLine($"The enemy attacks you with {Weapon.Name} for {AttackDamage} damage. You now have {player.Stats.GetStat(PlayerStatType.Health)} HP left.");
    }
}
