using Pentacle.CustomEvent.Args;
using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect.BasicTriggerEffects
{
    public class DoEffectOnArgsTargetTriggerEffect(TriggerEffect effect, int index = 0) : TriggerEffect
    {
        public TriggerEffect effect = effect;
        public int index = index;

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
