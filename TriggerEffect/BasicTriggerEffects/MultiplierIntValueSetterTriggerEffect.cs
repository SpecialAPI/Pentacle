using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect.BasicTriggerEffects
{
    /// <summary>
    /// A trigger effect that adds a MultiplyIntValueModifier to a value change exception stored in args.
    /// <para>See <see cref="ValueReferenceTools.TryGetValueChangeException"/> for more information.</para>
    /// </summary>
    /// <param name="multiplier">The multiplier value for the MultiplyIntValueModifier that this trigger effect adds.</param>
    public class MultiplierIntValueSetterTriggerEffect(int multiplier) : TriggerEffect
    {
        /// <summary>
        /// The multiplier value for the MultiplyIntValueModifier that this trigger effect adds.
        /// </summary>
        public int multiplier = multiplier;

        /// <inheritdoc/>
        public override void DoEffect(IUnit sender, object args, TriggeredEffect triggerInfo, TriggerEffectExtraInfo extraInfo)
        {
            if (!ValueReferenceTools.TryGetValueChangeException(args, out var exception))
                return;

            exception.AddModifier(new MultiplyIntValueModifier(exception.DamageDealt, multiplier));
        }
    }
}
