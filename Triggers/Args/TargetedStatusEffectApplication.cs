using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Args
{
    /// <summary>
    /// Provides information about a status effect being applied to a unit.
    /// <para>Sent as args by CustomTriggers.StatusEffectFirstAppliedToAnyone, CustomTriggers.StatusEffectAppliedToAnyone and CustomTriggers.StatusEffectIncreasedOnAnyone</para>
    /// </summary>
    /// <param name="unit">The unit that the status effect was applied to.</param>
    /// <param name="statusEffect">The applied status effect, as a StatusEffect_SO object.</param>
    /// <param name="amountToApply">The amount of the status effect that was applied.</param>
    public class TargetedStatusEffectApplication(IUnit unit, StatusEffect_SO statusEffect, int amountToApply) : IUnitHolder
    {
        /// <summary>
        /// The unit that the status effect was applied to.
        /// </summary>
        public readonly IUnit unit = unit;
        /// <summary>
        /// The applied status effect, as a StatusEffect_SO object.
        /// </summary>
        public readonly StatusEffect_SO statusEffect = statusEffect;
        /// <summary>
        /// The amount of the status effect that was applied.
        /// </summary>
        public readonly int amountToApply = amountToApply;

        IUnit IUnitHolder.Value
        {
            get => unit;
            set => PentacleLogger.LogWarning("TargetedStatusEffectApplication's Unit value is read-only.");
        }
        IUnit IUnitHolder.this[int index]
        {
            get => unit;
            set => PentacleLogger.LogWarning("TargetedStatusEffectApplication's Unit value is read-only.");
        }
    }
}
