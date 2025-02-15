using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    /// <summary>
    /// Extra information used for special damage.
    /// </summary>
    public class SpecialDamageInfo
    {
        /// <summary>
        /// Modifies how much damage modifiers apply to the damage. 0 means they apply normally, -100 means the damage isn't affected at all, 100 means the damage modifiers are doubled.
        /// </summary>
        public int ExtraDamageModifierPercentage;

        /// <summary>
        /// If true, the damage won't trigger "On Being Damaged" events.
        /// </summary>
        public bool DisableOnBeingDamagedCalls;
        /// <summary>
        /// If true, the damage won't trigger "On Damage" events.
        /// </summary>
        public bool DisableOnDamageCalls;

        /// <summary>
        /// If true, the damage will produce SpecialPigment instead of the unit's health color.
        /// </summary>
        public bool ProduceSpecialPigment;
        /// <summary>
        /// If ProduceSpecialPigment is true, pigment of this color will be produced instead of the unit's health color.
        /// </summary>
        public ManaColorSO SpecialPiment;
        /// <summary>
        /// If true, pigment will be produced even if it normally can't be produced.
        /// </summary>
        public bool ForcePigmentProduction;

        /// <summary>
        /// How much additional pigment the damage should produce. If SetsPigment is true, the amount of pigment produced is set to this amount instead.
        /// </summary>
        public int ExtraPigment;
        /// <summary>
        /// If true, ExtraPigment will be the exact amount of pigment produced, instead of an addition.
        /// </summary>
        public bool SetsPigment;
    }
}
