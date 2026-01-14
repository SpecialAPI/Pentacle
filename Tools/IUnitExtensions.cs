using Pentacle.Misc;
using FMOD.Studio;
using System;
using System.Collections.Generic;
using System.Text;
using Pentacle.Internal;

namespace Pentacle.Tools
{
    /// <summary>
    /// Static class that proides IUnit-related extensions and tools.
    /// </summary>
    public static class IUnitExtensions
    {
        /// <summary>
        /// Returns the list of this unit's combat abilities.
        /// </summary>
        /// <param name="unit">The unit to get the abilities from.</param>
        /// <returns>A list of the input unit's abilities.</returns>
        public static List<CombatAbility> Abilities(this IUnit unit)
        {
            if (unit is CharacterCombat cc)
                return cc.CombatAbilities;

            else if (unit is EnemyCombat ec)
                return ec.Abilities;

            return [];
        }

        /// <summary>
        /// Returns the list of this unit's extra abilities.
        /// </summary>
        /// <param name="unit">The unit to get the extra abilities from.</param>
        /// <returns>A list of the input unit's extra abilities.</returns>
        public static List<ExtraAbilityInfo> ExtraAbilities(this IUnit unit)
        {
            if (unit is CharacterCombat cc)
                return cc.ExtraAbilities;

            else if (unit is EnemyCombat ec)
                return ec.ExtraAbilities;

            return [];
        }

        /// <summary>
        /// Gets the slot ID of the rightmost slot occupied by this unit.
        /// </summary>
        /// <param name="u">The unit to get the last slot ID from.</param>
        /// <returns>The slot ID of the rightmost slot occupied by the input unit.</returns>
        public static int LastSlotId(this IUnit u)
        {
            return u.SlotID + u.Size - 1;
        }

        /// <summary>
        /// Gets the distance between two units.
        /// <para>If the first unit is on the right of the second unit, this returns the difference between the slot IDs of the first unit's leftmost slot and the second unit's rightmost slot.</para>
        /// <para>If the first unit is on the left of the second unit, this returns the difference between the slot IDs of the first unit's rightmost slot and the second unit's leftmost slot.</para>
        /// <para>If the two units are opposing, this returns 0. See <see cref="IsOpposing(IUnit, IUnit)"/>.</para>
        /// </summary>
        /// <param name="a">The first unit.</param>
        /// <param name="b">The second unit.</param>
        /// <returns>The distance between the two units.</returns>
        public static int DistanceBetween(this IUnit a, IUnit b)
        {
            if (a == null || b == null)
                return 0;

            var aFirst = a.SlotID;
            var aLast = a.LastSlotId();

            var bFirst = b.SlotID;
            var bLast = b.LastSlotId();

            if (bLast < aFirst && bFirst <= aLast)
                return Mathf.Abs(aFirst - bLast);

            else if (bFirst > aLast && bLast >= aFirst)
                return Mathf.Abs(bFirst - aLast);

            return 0;
        }

        /// <summary>
        /// Checks if two units are opposing. The units are considered opposing if at least 1 of the slots occupied by the first unit is in front of a slot occupied by the second unit.
        /// </summary>
        /// <param name="a">The first unit.</param>
        /// <param name="b">The second unit.</param>
        /// <returns>True if the units are opposing, false otherwise.</returns>
        public static bool IsOpposing(this IUnit a, IUnit b)
        {
            if (a == null || b == null || a.IsUnitCharacter == b.IsUnitCharacter)
                return false;

            var aFirst = a.SlotID;
            var aLast = a.LastSlotId();

            var bFirst = b.SlotID;
            var bLast = b.LastSlotId();

            return aFirst <= bLast && bFirst <= aLast;
        }

        /// <summary>
        /// Checks if the first unit is to the right of the second unit. This returns true if the first unit's leftmost slot is to the right of the second unit's rightmost slot.
        /// </summary>
        /// <param name="a">The first unit.</param>
        /// <param name="b">The second unit.</param>
        /// <returns>True if the first unit is to the right of the second unit, false otherwise.</returns>
        public static bool IsRightOf(this IUnit a, IUnit b)
        {
            if (a == null || b == null)
                return false;

            var aFirst = a.SlotID;
            var bLast = b.LastSlotId();

            return bLast < aFirst;
        }

        /// <summary>
        /// Checks if the first unit is to the left of the second unit. This returns true if the first unit's rightmost slot is to the left of the second unit's leftmost slot.
        /// </summary>
        /// <param name="a">The first unit.</param>
        /// <param name="b">The second unit.</param>
        /// <returns>True if the first unit is to the left of the second unit, false otherwise.</returns>
        public static bool IsLeftOf(this IUnit a, IUnit b)
        {
            if (a == null || b == null)
                return false;

            var bFirst = b.SlotID;
            var aLast = a.LastSlotId();

            return bFirst > aLast;
        }

        /// <summary>
        /// Attempts to move a unit by 1 slot.
        /// </summary>
        /// <param name="unit">The unit that should be moved.</param>
        /// <param name="toRight">Determines whether this will move the unit to the right or left.</param>
        /// <returns>True if the movement was successful, false otherwise.</returns>
        public static bool TryMoveUnit(this IUnit unit, bool toRight)
        {
            if (unit == null)
                return false;

            var move = toRight ? (unit.IsUnitCharacter ? 1 : unit.Size) : -1;
            var slots = CombatManager.Instance._stats.combatSlots;

            if (unit.IsUnitCharacter)
                return unit.SlotID + move >= 0 && unit.SlotID + move < slots.CharacterSlots.Length && slots.SwapCharacters(unit.SlotID, unit.SlotID + move, true);

            return slots.CanEnemiesSwap(unit.SlotID, unit.SlotID + move, out var firstSlotSwap, out var secondSlotSwap) && slots.SwapEnemies(unit.SlotID, firstSlotSwap, unit.SlotID + move, secondSlotSwap, true);
        }

        /// <summary>
        /// Attempts to remove a character's held item in combat. This will not destroy or unequip the item outside of combat.
        /// <para>This method does nothing for enemies.</para>
        /// </summary>
        /// <param name="unit">The unit whose item should be unequipped.</param>
        /// <returns>True if the item was successfully unequipped, false otherwise. Always returns false for enemies.</returns>
        public static bool TryUnequipItem(this IUnit unit)
        {
            if(!unit.HasUsableItem)
                return false;

            if(unit is not CharacterCombat cc)
                return false;

            var itm = cc.HeldItem;
            var id = cc.ID;

            cc.DettachWearable();
            itm.OnCharacterDettached(cc.CharacterWearableModifiers);
            cc.RemoveAndDisconnectAllItemExtraPassiveAbilities();

            cc.HeldItem = null;
            cc.IsWearableConsumed = false;

            cc.ItemExtraPassives.Clear();
            cc.ClampedRank = cc.Character.ClampRank(cc.Rank + cc.CharacterWearableModifiers.RankModifier);
            cc.CurrencyMultiplier = cc.CharacterWearableModifiers.CurrencyMultiplierModifier;

            var prevMaximumHealth = cc.MaximumHealth;
            cc.MaximumHealth = cc.Character.GetMaxHealth(cc.ClampedRank);
            cc.MaximumHealth = Mathf.Max(1, cc.CharacterWearableModifiers.MaximumHealthModifier + cc.MaximumHealth);

            var prevHealthColor = cc.HealthColor;
            cc.HealthColor = (cc.CharacterWearableModifiers.HasHealthColorModifier ? cc.CharacterWearableModifiers.HealthColorModifier : cc.Character.healthColor);
            cc.CurrentHealth = Mathf.Min(cc.CurrentHealth, cc.MaximumHealth);

            cc.SetUpDefaultAbilities(true);

            CombatManager.Instance.AddUIAction(new CharacterItemChangeUIAction(id, cc.HeldItem, cc.IsWearableConsumed));
            CombatManager.Instance.AddUIAction(new CharacterPassiveAbilityChangeUIAction(id, [.. cc.PassiveAbilities], cc.CanSwapNoTrigger, cc.CanUseAbilitiesNoTrigger));

            if (prevMaximumHealth != cc.MaximumHealth)
                CombatManager.Instance.AddUIAction(new CharacterMaximumHealthChangeUIAction(id, cc.CurrentHealth, cc.MaximumHealth, cc.MaximumHealth - prevMaximumHealth));
            if (prevHealthColor != cc.HealthColor)
                CombatManager.Instance.AddUIAction(new CharacterHealthColorChangeUIAction(id, cc.HealthColor));

            return true;
        }

        /// <summary>
        /// An extended version of <see cref="IUnit.Damage(int, IUnit, string, int, bool, bool, bool, string)"/>. This method can be given extra information about how the damage should be dealt.
        /// </summary>
        /// <param name="u">The unit that will be damaged.</param>
        /// <param name="amount">The amount of damage to deal.</param>
        /// <param name="killer">The unit dealing the damage.</param>
        /// <param name="deathType">The damage's death type.</param>
        /// <param name="sinfo">Extra information about how the damage should be dealt.</param>
        /// <param name="targetSlotOffset">The offset of the hit slot from the unit's first slot. If this number is negative, the damage will be applied to all of the unit's slots.</param>
        /// <param name="addHealthMana">If false, the damage won't produce pigment.</param>
        /// <param name="directDamage">If true, the damage will be considered direct. If false, the damage will be considered indirect.</param>
        /// <param name="ignoresShield">If true, the damage will ignore shield on the unit's slots.</param>
        /// <param name="damageId">The ID of the damage's type. If this argument is empty, the damage's type will be determined by how much damage is dealt instead.</param>
        /// <returns>A DamageInfo that stores how much damage was dealt and whether it was fatal or not.</returns>
        public static DamageInfo SpecialDamage(this IUnit u, int amount, IUnit killer, SpecialDamageInfo sinfo, string deathType, int targetSlotOffset = -1, bool addHealthMana = true, bool directDamage = true, bool ignoresShield = false, string damageId = "")
        {
            if (!DamageReversePatches.ReversePatchesDone)
            {
                PentacleLogger.LogError($"Trying to do SpecialDamage before the Damage reverse patches are done. This should not be happening.");
                return default;
            }

            if (u is CharacterCombat cc)
                return DamageReversePatches.SpecialDamage_Characters_ReversePatch(cc, amount, killer, deathType, targetSlotOffset, addHealthMana, directDamage, ignoresShield, damageId, sinfo);
            else if (u is EnemyCombat ec)
                return DamageReversePatches.SpecialDamage_Enemies_ReversePatch(ec, amount, killer, deathType, targetSlotOffset, addHealthMana, directDamage, ignoresShield, damageId, sinfo);

            PentacleLogger.LogError("Trying to do SpecialDanage to a unit that is neither an enemy nor a party member.");
            return default;
        }
    }
}
