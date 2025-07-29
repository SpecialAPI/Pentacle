using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect.BasicTriggerEffects
{
    public class StringHolderSetterTriggerEffect(string value, int index = 0) : TriggerEffect
    {
        public string value = value;
        public int index = index;

        public override void DoEffect(IUnit sender, object args, TriggeredEffect triggerInfo, TriggerEffectExtraInfo extraInfo)
        {
            if (!ValueReferenceTools.TryGetStringHolder(args, out var stringRef))
                return;

            stringRef[index] = value;
        }
    }
}
