using Pentacle.Triggers;
using Pentacle.Triggers.Args;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    /// <summary>
    /// Static class that provides extension methods for CombatManager, CombatStats and SlotsCombat.
    /// </summary>
    public static class CombatExtensions
    {
        /// <summary>
        /// Returns a list of all units in combat. Charaters go before enemies, and both characters and enemies are ordered by their combat IDs.
        /// </summary>
        /// <param name="stats">Combat stats to get the units from.</param>
        /// <returns>A list of all units in combat.</returns>
        public static List<IUnit> Units(this CombatStats stats)
        {
            var list = new List<IUnit>();

            list.AddRange(stats.Characters.Values);
            list.AddRange(stats.Enemies.Values);

            return list;
        }

        /// <summary>
        /// Gets all units on the field. Charaters go before enemies, and both characters and enemies are ordered by their combat IDs.
        /// </summary>
        /// <param name="stats">Combat stats to get the units from.</param>
        /// <returns>A list of all units on field.</returns>
        public static List<IUnit> UnitsOnField(this CombatStats stats)
        {
            var list = new List<IUnit>();

            list.AddRange(stats.CharactersOnField.Values);
            list.AddRange(stats.EnemiesOnField.Values);

            return list;
        }

        /// <summary>
        /// Tries to get a unit in combat.
        /// </summary>
        /// <param name="stats">The stats to get the unit from.</param>
        /// <param name="id">The combat ID of the unit to get.</param>
        /// <param name="character">If true, this method will get a character. IF false, it will get an enemy.</param>
        /// <param name="unit">If successful, outputs the found unit. Otherwise, outputs null.</param>
        /// <returns>True if the unit was successfully found, false otherwise.</returns>
        public static bool TryGetUnit(this CombatStats stats, int id, bool character, out IUnit unit)
        {
            if (character && stats.Characters.TryGetValue(id, out var cc))
            {
                unit = cc;

                return true;
            }

            if (!character && stats.Enemies.TryGetValue(id, out var ec))
            {
                unit = ec;

                return true;
            }

            unit = null;
            return false;
        }

        /// <summary>
        /// Tries to get a unit on the field..
        /// </summary>
        /// <param name="stats">The stats to get the unit from.</param>
        /// <param name="id">The combat ID of the unit to get.</param>
        /// <param name="character">If true, this method will get a character. IF false, it will get an enemy.</param>
        /// <param name="unit">If successful, outputs the found unit. Otherwise, outputs null.</param>
        /// <returns>True if the unit was successfully found, false otherwise.</returns>
        public static bool TryGetUnitOnField(this CombatStats stats, int id, bool character, out IUnit unit)
        {
            if (character && stats.CharactersOnField.TryGetValue(id, out var cc))
            {
                unit = cc;

                return true;
            }

            if (!character && stats.EnemiesOnField.TryGetValue(id, out var ec))
            {
                unit = ec;

                return true;
            }

            unit = null;
            return false;
        }

        /// <summary>
        /// Tries to get a slot in combat.
        /// </summary>
        /// <param name="slots">The SlotsCombat to get the slot from.</param>
        /// <param name="slotId">The slot ID of the slot to get.</param>
        /// <param name="isCharacterSlot">If true, this method will get a slot from the character side. If false, it will get a slot from the enemy side.</param>
        /// <param name="slot">If successful, outputs the found slot. Otherwise, outputs null.</param>
        /// <returns>True if the slot was successfully found, false otherwise.</returns>
        public static bool TryGetSlot(this SlotsCombat slots, int slotId, bool isCharacterSlot, out CombatSlot slot)
        {
            var list = isCharacterSlot ? slots.CharacterSlots : slots.EnemySlots;

            if (list == null || slotId < 0 | slotId >= list.Length)
            {
                slot = null;

                return false;
            }

            slot = list[slotId];
            return true;
        }

        /// <summary>
        /// Immediately rolls the lucky pigment chance.
        /// </summary>
        /// <param name="stats">Combat stats.</param>
        /// <returns>How much lucky pigment was produced.</returns>
        public static int TriggerLuckyPigment(this CombatStats stats)
        {
            var emptySlots = stats.MainManaBar.EmptySlotsCount;

            if (emptySlots <= 0)
                return 0;

            var chance = stats.LuckyManaPercentage;
            var amount = Mathf.Min(stats.LuckyManaAmount, emptySlots);
            var maxRoll = CombatData.LuckyPigmentMaxPercentage;

            if (Random.Range(0, maxRoll) >= chance)
            {
                foreach (var u in stats.UnitsOnField())
                    CombatManager.Instance.PostNotification(CustomTriggers.OnLuckyPigmentFailure, u, null);

                return 0;
            }

            CombatManager.Instance.AddUIAction(new AddLuckyManaUIAction());
            var jumpInfo = new JumpAnimationInformation(true, stats.combatUI.LuckyManaPosition);
            stats.MainManaBar.AddManaAmount(stats.LuckyManaColorOptions[stats.SelectedLuckyColor], amount, jumpInfo);

            var luckyPigmentRef = new OnLuckyPigmentSuccessReference(amount);
            foreach (var u in stats.UnitsOnField())
                CombatManager.Instance.PostNotification(CustomTriggers.OnLuckyPigmentSuccess, u, luckyPigmentRef);

            return amount;
        }

        /// <summary>
        /// Attempts to immedately trigger overflow.
        /// </summary>
        /// <param name="stats">Combat stats.</param>
        /// <returns>True if the overflow was successfully triggered, false otherwise. If overflow is blocked (for example by using Blue Bible or Indulgence) it will be counted as unsuccessful.</returns>
        public static bool TryTriggerOverflow(this CombatStats stats)
        {
            if (stats.overflowMana.OverflowManaAmount <= 0)
                return false;

            var chars = 0;
            var overflowAbsorbers = new HashSet<int>();
            var shouldTrigger = true;

            foreach (var ch in stats.CharactersOnField.Values)
            {
                if (!ch.IsAlive)
                    continue;

                chars++;

                if (ch.ShouldAbsorbOverflow)
                    overflowAbsorbers.Add(ch.ID);

                if (shouldTrigger)
                    shouldTrigger = shouldTrigger && ch.CanOverflowTrigger;
            }

            if (!shouldTrigger)
                return false;

            var damagePercent = stats.overflowMana.DepleteAllStoredMana() * CombatData.CharDmgPercPerPigmentOverflow;

            if (overflowAbsorbers.Count > 0)
                damagePercent = damagePercent * chars / overflowAbsorbers.Count;

            foreach (var ch in stats.CharactersOnField.Values)
            {
                if (!ch.IsAlive || damagePercent <= 0)
                    continue;

                if (overflowAbsorbers.Count > 0 && !overflowAbsorbers.Contains(ch.ID))
                    continue;

                var dmg = ch.CalculatePercentualAmount(damagePercent);
                ch.ManaDamage(dmg, true, DeathType_GameIDs.Overflow.ToString());
            }

            return true;
        }
    }
}
