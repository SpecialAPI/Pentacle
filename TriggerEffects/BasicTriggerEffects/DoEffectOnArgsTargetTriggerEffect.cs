using Pentacle.CustomEvent.Args;
using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffects.BasicTriggerEffects
{
    /// <summary>
    /// A trigger effect that triggers another trigger effect on the unit value of a unit holder stored in args.
    /// <para>The second trigger effect gets the same arguments as this trigger effect, with the exception of the sender unit.</para>
    /// <para>See <see cref="ValueReferenceTools.TryGetUnitHolder"/> for more information.</para>
    /// </summary>
    /// <param name="effect">The trigger effect that should be triggered on the unit value of the unit holder.</param>
    /// <param name="index">The index for the unit value that the second trigger effect should be performed on.<para>See <see cref="ValueReferenceTools.TryGetUnitHolder"/> for more information.</para></param>
    public class DoEffectOnArgsTargetTriggerEffect(TriggerEffect effect, int index = 0) : TriggerEffect
    {
        /// <summary>
        /// The trigger effect that should be triggered on the value of the unit holder.
        /// </summary>
        public TriggerEffect effect = effect;
        /// <summary>
        /// The index for the value that the second trigger effect should be performed on.<para>See <see cref="ValueReferenceTools.TryGetUnitHolder"/> for more information.</para>
        /// </summary>
        public int index = index;

        /// <inheritdoc/>
        public override void DoEffect(IUnit sender, object args, TriggeredEffect triggerInfo, TriggerEffectExtraInfo extraInfo)
        {
            if(effect == null)
                return;

            if (!ValueReferenceTools.TryGetUnitHolder(args, out var hold) || hold[index] is not IUnit unit)
                return;

            effect.DoEffect(unit, args, triggerInfo, extraInfo);
        }
    }
}
