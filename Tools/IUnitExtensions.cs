using Pentacle.Misc;
using FMOD.Studio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    /// <summary>
    /// IUnit-related extension methods.
    /// </summary>
    public static class IUnitExtensions
    {
        /// <summary>
        /// Gets the abilities from a generic unit.
        /// </summary>
        /// <param name="unit">The unit to get the abilities from.</param>
        /// <returns>A list of the unit's abilities.</returns>
        public static List<CombatAbility> Abilities(this IUnit unit)
        {
            if (unit is CharacterCombat cc)
                return cc.CombatAbilities;

            else if (unit is EnemyCombat ec)
                return ec.Abilities;

            return [];
        }

        public static List<ExtraAbilityInfo> ExtraAbilities(this IUnit unit)
        {
            if (unit is CharacterCombat cc)
                return cc.ExtraAbilities;

            else if (unit is EnemyCombat ec)
                return ec.ExtraAbilities;

            return [];
        }

        /// <summary>
        /// Gets the slot id of the last slot occupied by a unit.
        /// </summary>
        /// <param name="u">The target unit.</param>
        /// <returns>The unit's last slot id.</returns>
        public static int LastSlotId(this IUnit u)
        {
            return u.SlotID + u.Size - 1;
        }

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

        public static bool IsRightOf(this IUnit a, IUnit b)
        {
            if (a == null || b == null)
                return false;

            var aFirst = a.SlotID;
            var bLast = b.LastSlotId();

            return bLast < aFirst;
        }

        public static bool IsLeftOf(this IUnit a, IUnit b)
        {
            if (a == null || b == null)
                return false;

            var bFirst = b.SlotID;
            var aLast = a.LastSlotId();

            return bFirst > aLast;
        }

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
        /// Damages the given unit. Can be given extra info to further configure the damage.
        /// </summary>
        /// <param name="u">The unit to damage.</param>
        /// <param name="amount">How much damage will be dealt.</param>
        /// <param name="killer">The unit dealing the damage.</param>
        /// <param name="deathType">The damage's death type.</param>
        /// <param name="sinfo">Extra damage info.</param>
        /// <param name="targetSlotOffset">The offset of the hit slot from the unit's first slot.</param>
        /// <param name="addHealthMana">Should this damage produce pigment?</param>
        /// <param name="directDamage">Is this damage direct or indirect?</param>
        /// <param name="ignoresShield">Does this damage ignore shield?</param>
        /// <param name="damageId">Special id for the damage. If empty, will be automatically set based on how much damage was dealt.</param>
        /// <returns>The info for the dealt damage.</returns>
        public static DamageInfo SpecialDamage(this IUnit u, int amount, IUnit killer, string deathType, SpecialDamageInfo sinfo, int targetSlotOffset = -1, bool addHealthMana = true, bool directDamage = true, bool ignoresShield = false, string damageId = "")
        {
            // If unit type is not recognized or the special damage info is null, do regular damage.
            if (u is not CharacterCombat and not EnemyCombat)
                return u.Damage(amount, killer, deathType, targetSlotOffset, addHealthMana, directDamage, ignoresShield, damageId);

            var firstSlot = u.SlotID;
            var lastSlot = u.LastSlotId();

            if (targetSlotOffset >= 0)
            {
                targetSlotOffset = Mathf.Clamp(u.SlotID + targetSlotOffset, firstSlot, lastSlot);
                firstSlot = targetSlotOffset;
                lastSlot = targetSlotOffset;
            }

            // Create a value change exception for received damage.
            var ex = new DamageReceivedValueChangeException(amount, damageId, directDamage, ignoresShield, firstSlot, lastSlot, killer, u);

            // Only post the OnBeingDamaged notification if the special damage allows OnBeingDamaged calls.
            if (!sinfo.DisableOnBeingDamagedCalls)
                CombatManager.Instance.PostNotification(TriggerCalls.OnBeingDamaged.ToString(), u, ex);

            // Get the modified value from the exception.
            var modifiedValue = ex.GetModifiedValue();

            // Change the modified value based on how much damage modifiers should affect the damage.
            if (sinfo.ExtraDamageModifierPercentage != 0)
                modifiedValue = (int)Mathf.LerpUnclamped(amount, modifiedValue, 1f + (sinfo.ExtraDamageModifierPercentage / 100f));

            // Damage can't be negative.
            modifiedValue = Mathf.Max(modifiedValue, 0);

            // Process the UnitDamaged action
            if (killer != null && !killer.Equals(null))
                CombatManager.Instance.ProcessImmediateAction(new UnitDamagedImmediateAction(modifiedValue, killer.IsUnitCharacter));

            // Get the new health and the damage dealt.
            var newHealth = Mathf.Max(u.CurrentHealth - modifiedValue, 0);
            var damageDealt = u.CurrentHealth - newHealth;

            if (damageDealt != 0)
            {
                // Set the unit's health
                if (u is CharacterCombat cc)
                {
                    cc.GetHit();
                    cc.CurrentHealth = newHealth;
                }
                else if (u is EnemyCombat ec)
                    ec.CurrentHealth = newHealth;

                // If a damage id isn't given, get it from the damage amount.
                if (damageId.Equals(""))
                    damageId = Utils.GetBasicDamageIDFromAmount(modifiedValue);

                // Make the UI show the damage.
                if (u is CharacterCombat)
                    CombatManager.Instance.AddUIAction(new CharacterDamagedUIAction(u.ID, u.CurrentHealth, u.MaximumHealth, modifiedValue, damageId));
                else if (u is EnemyCombat)
                    CombatManager.Instance.AddUIAction(new EnemyDamagedUIAction(u.ID, u.CurrentHealth, u.MaximumHealth, modifiedValue, damageId));

                if (addHealthMana)
                {
                    // Get the unit's health color as a default.
                    var col = u.HealthColor;

                    // If the special damgae produces special pigment, produce that color pigment instead.
                    if (sinfo.ProduceSpecialPigment && sinfo.SpecialPiment != null)
                        col = sinfo.SpecialPiment;

                    // Get the base amount of produced pigment.
                    var amt = u is CharacterCombat ? CombatData.CharacterPigmentAmount : CombatData.EnemyPigmentAmount;

                    // Process the special damage's pigment amount modifiers.
                    if (sinfo.SetsPigment)
                        amt = sinfo.ExtraPigment;
                    else
                        amt += sinfo.ExtraPigment;

                    // Produce the pigment.
                    if (col != null && amt > 0)
                        CombatManager.Instance.ProcessImmediateAction(sinfo.ForcePigmentProduction ? new ForceAddPigmentAction(col, amt, u.IsUnitCharacter, u.ID) : new AddManaToManaBarAction(col, amt, u.IsUnitCharacter, u.ID));
                }

                // Only post the OnDamaged notifications if the special damage allows OnDamaged calls.
                if (!sinfo.DisableOnDamageCalls)
                {
                    CombatManager.Instance.PostNotification(TriggerCalls.OnDamaged.ToString(), u, new IntegerReference(damageDealt));

                    if (directDamage)
                        CombatManager.Instance.PostNotification(TriggerCalls.OnDirectDamaged.ToString(), u, new IntegerReference(damageDealt));
                }
            }

            else if (ex == null || !ex.ShouldIgnoreUI)
            {
                // If the unit wasn't damaged, make the UI show that.
                if (u is CharacterCombat)
                    CombatManager.Instance.AddUIAction(new CharacterNotDamagedUIAction(u.ID, CombatType_GameIDs.Dmg_Weak.ToString()));
                else if (u is EnemyCombat)
                    CombatManager.Instance.AddUIAction(new EnemyNotDamagedUIAction(u.ID));
            }

            // Check if the damage killed the unit.
            var killed = u.CurrentHealth == 0 && damageDealt != 0;

            if (killed)
            {
                // Trigger the unit's death action.
                if (u is CharacterCombat)
                    CombatManager.Instance.AddSubAction(new CharacterDeathAction(u.ID, killer, deathType));
                else if (u is EnemyCombat)
                    CombatManager.Instance.AddSubAction(new EnemyDeathAction(u.ID, killer, deathType));
            }

            // Return the info of the dealt damage.
            return new(damageDealt, killed);
        }

        public static int Threaten(this IUnit u, int amount, IUnit killer, int targetSlotOffset = -1, bool directDamage = true, bool ignoresShield = false, string specialDamage = "")
        {
            var firstSlot = u.SlotID;
            var lastSlot = u.LastSlotId();

            if (targetSlotOffset >= 0)
            {
                targetSlotOffset = Mathf.Clamp(u.SlotID + targetSlotOffset, firstSlot, lastSlot);
                firstSlot = targetSlotOffset;
                lastSlot = targetSlotOffset;
            }

            var ex = new DamageReceivedValueChangeException(amount, specialDamage, directDamage, ignoresShield, firstSlot, lastSlot, killer, u);

            CombatManager.Instance.PostNotification(TriggerCalls.OnBeingDamaged.ToString(), u, ex);
            amount = ex.GetModifiedValue();

            var newHealth = Mathf.Max(u.CurrentHealth - amount, 0);
            var damageDealt = u.CurrentHealth - newHealth;

            if(damageDealt > 0)
            {
                if (specialDamage == "")
                    specialDamage = Utils.GetBasicDamageIDFromAmount(amount);

                if (u is CharacterCombat)
                    CombatManager.Instance.AddUIAction(new CharacterDamagedUIAction(u.ID, u.CurrentHealth, u.MaximumHealth, amount, CombatType_GameIDs.Dmg_Weak.ToString()));
                else if (u is EnemyCombat)
                    CombatManager.Instance.AddUIAction(new EnemyDamagedUIAction(u.ID, u.CurrentHealth, u.MaximumHealth, amount, CombatType_GameIDs.Dmg_Weak.ToString()));

                CombatManager.Instance.PostNotification(TriggerCalls.OnDamaged.ToString(), u, new IntegerReference(damageDealt));

                if (directDamage)
                    CombatManager.Instance.PostNotification(TriggerCalls.OnDirectDamaged.ToString(), u, new IntegerReference(damageDealt));
            }
            else
            {
                if (u is CharacterCombat)
                    CombatManager.Instance.AddUIAction(new CharacterNotDamagedUIAction(u.ID, CombatType_GameIDs.Dmg_Weak.ToString()));
                else if (u is EnemyCombat)
                    CombatManager.Instance.AddUIAction(new EnemyNotDamagedUIAction(u.ID));
            }

            return damageDealt;
        }
    }
}
