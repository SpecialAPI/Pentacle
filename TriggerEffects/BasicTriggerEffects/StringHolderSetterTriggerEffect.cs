using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffects.BasicTriggerEffects
{
    /// <summary>
    /// A trigger effect that sets the value of a string holder stored in args.
    /// <para>See <see cref="ValueReferenceTools.TryGetStringHolder"/> for more information.</para>
    /// </summary>
    /// <param name="value">The new value for the string holder.</param>
    /// <param name="index">The index for the value that should be set.<para>See <see cref="ValueReferenceTools.TryGetStringHolder"/> for more information.</para></param>
    public class StringHolderSetterTriggerEffect(string value, int index = 0) : TriggerEffect
    {
        /// <summary>
        /// The new value for the string holder.
        /// </summary>
        public string value = value;
        /// <summary>
        /// The index for the value that should be set.
        /// <para>See <see cref="ValueReferenceTools.TryGetStringHolder"/> for more information.</para>
        /// </summary>
        public int index = index;

        /// <inheritdoc/>
        public override void DoEffect(IUnit sender, object args, TriggerEffectInfo triggerInfo, TriggerEffectActivationExtraInfo extraInfo)
        {
            if (!ValueReferenceTools.TryGetStringHolder(args, out var stringRef))
                return;

            stringRef[index] = value;
        }
    }
}
