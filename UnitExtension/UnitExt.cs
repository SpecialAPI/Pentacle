using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.UnitExtension
{
    /// <summary>
    /// Extended variables for a IUnit.
    /// </summary>
    public class UnitExt
    {
        /// <summary>
        /// The pigment colors used by this unit for the ability they're currently performed. This list is always empty for enemies.
        /// </summary>
        public readonly List<ManaColorSO> PigmentUsedForAbility = [];

        /// <summary>
        /// A list of this unit's health color options
        /// </summary>
        public readonly List<ManaColorSO> HealthColors;
        /// <summary>
        /// This unit's current health color option.
        /// </summary>
        public int HealthColorIndex = 0;

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
