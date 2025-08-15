using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    /// <summary>
    /// A class that provides extra damage information. Used by Pentacle's <see cref="IUnitExtensions.SpecialDamage"/> extension.
    /// </summary>
    public class SpecialDamageInfo
    {
        /// <summary>
        /// Modifies how much OnBeingDamaged damage modifiers affect the dealt damage. At 0 (the default value) damage modifiers affect the damage normally, at -100 damage modifiers don't affect the damage at all and at 100 damage modifiers are twice as effective (e.g Scars will increase damage by 2 per stack).
        /// <para>More specifically, the final damage dealt is determined using this formula:</para>
        /// <code>finalDamage = modifiedDamage + (modifiedDamage - unmodifiedDamage) * (ExtraDamageModifierPercentage / 100)</code>
        /// </summary>
        public int ExtraDamageModifierPercentage;

        /// <summary>
        /// If true, the damage will not trigger the OnBeingDamaged trigger call.
        /// </summary>
        public bool DisableOnBeingDamagedCalls;
        /// <summary>
        /// If true, the damage will not trigger OnDamaged, OnBeingDamaged, OnIndirect damaged and other trigger calls that are triggered when a unit is damaged.
        /// </summary>
        public bool DisableOnDamageCalls;

        /// <summary>
        /// If true, the damage will produce the pigment color stored in <see cref="SpecialPigment"/> instead of the damage target's health color.
        /// </summary>
        public bool ProduceSpecialPigment;
        /// <summary>
        /// If <see cref="ProduceSpecialPigment"/> is true, the damage will produce pigment of this color instead of the damage target's health color.
        /// <para>If <see cref="ProduceSpecialPigment"/> is true and the value of this field is null, the damage will not produce any pigment at all.</para>
        /// </summary>
        public ManaColorSO SpecialPigment;
        /// <summary>
        /// If true, the damage will produce pigment colors that can't be normally produced, such as grey pigment.
        /// </summary>
        public bool ForcePigmentProduction;

        /// <summary>
        /// Determines how much additional pigment the damage should produce. If <see cref="SetsPigment"/> is true, the amount of pigment produced is set to this amount instead.
        /// </summary>
        public int ExtraPigment;
        /// <summary>
        /// If true, <see cref="ExtraPigment"/> will determine the exact amount of pigment produced, instead of adding to that amount.
        /// </summary>
        public bool SetsPigment;
    }
}
