using Pentacle.HiddenPassiveEffects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.UnitExtension
{
    /// <summary>
    /// Class that provides extended variables for a unit in combat.
    /// </summary>
    public class UnitExt
    {
        /// <summary>
        /// The pigment colors used by this unit for the ability they're currently performing. This list is always empty for enemies.
        /// </summary>
        public readonly List<ManaColorSO> PigmentUsedForAbility = [];

        /// <summary>
        /// A list of this unit's health color options
        /// </summary>
        public readonly List<ManaColorSO> HealthColors;
        /// <summary>
        /// The index of this unit's current health color option.
        /// </summary>
        public int HealthColorIndex = 0;

        /// <summary>
        /// A list of his unit's hidden passive effects.
        /// </summary>
        public readonly List<HiddenPassiveEffectSO> HiddenPassiveEffects = [];

        internal UnitExt(IUnit unit)
        {
            HealthColors = [unit.HealthColor];
        }

        /// <summary>
        /// Adds a new health color option to this unit.
        /// </summary>
        /// <param name="color">The new health color option to add.</param>
        public void AddHealthColor(ManaColorSO color)
        {
            HealthColors.Add(color);
        }
    }
}
