using Pentacle.CustomEvent;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    /// <summary>
    /// Combat-related extension methods.
    /// </summary>
    public static class CombatExtensions
    {
        /// <summary>
        /// Gets all units in combat.
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
        /// Gets all units on field.
        /// </summary>
        /// <param name="stats">Combat stats to get the units from</param>
        /// <returns>A list of all units on field.</returns>
        public static List<IUnit> UnitsOnField(this CombatStats stats)
        {
            var list = new List<IUnit>();

            list.AddRange(stats.CharactersOnField.Values);
            list.AddRange(stats.EnemiesOnField.Values);

            return list;
        }

        /// <summary>
        /// Tries to get a unit with a given id.
        /// </summary>
        /// <param name="stats">The stats to get the unit from.</param>
        /// <param name="id">The id of the unit to get.</param>
        /// <param name="character">Is the unit a character?</param>
        /// <param name="unit">If successful, outputs the found unit. Otherwise, outputs null.</param>
        /// <returns>True if successful, false otherwise.</returns>
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
        /// Tries to get a unit on field with a given id.
        /// </summary>
        /// <param name="stats">The stats to get the unit from.</param>
        /// <param name="id">The id of the unit to get.</param>
        /// <param name="character">Is the unit a character?</param>
        /// <param name="unit">If successful, outputs the found unit. Otherwise, outputs null.</param>
        /// <returns>True if successful, false otherwise.</returns>
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

            var intRef = new IntegerReference(amount);
            foreach (var u in stats.UnitsOnField())
                CombatManager.Instance.PostNotification(CustomTriggers.OnLuckyPigmentSuccess, u, intRef);

            return amount;
        }

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
