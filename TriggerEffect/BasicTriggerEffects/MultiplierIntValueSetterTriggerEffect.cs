using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect.BasicTriggerEffects
{
    public class MultiplierIntValueSetterTriggerEffect(int multiplier) : TriggerEffect
    {
        public int multiplier = multiplier;

        public override void DoEffect(IUnit sender, object args, TriggeredEffect triggerInfo, TriggerEffectExtraInfo extraInfo)
        {
            if (!ValueReferenceTools.TryGetValueChangeException(args, out var exception))
                return;

            exception.AddModifier(new MultiplyIntValueModifier(exception.DamageDealt, multiplier));
        }
    }
}
