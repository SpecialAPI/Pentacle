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
        /// A list of his unit's hidden passive effects.
        /// </summary>
        public readonly List<HiddenPassiveEffectSO> HiddenPassiveEffects = [];

        internal UnitExt(IUnit _)
        {
        }
    }
}
