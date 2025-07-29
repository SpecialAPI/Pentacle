using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect.BasicTriggerEffects
{
    public class BoolHolderSetterTriggerffect(bool value, int index = 0) : TriggerEffect
    {
        public bool value = value;
        public int index = index;

        public override void DoEffect(IUnit sender, object args, TriggeredEffect triggerInfo, TriggerEffectExtraInfo extraInfo)
        {
            if (!ValueReferenceTools.TryGetBoolHolder(args, out var boolHolder))
                return;

            boolHolder[index] = value;
        }
    }
}
