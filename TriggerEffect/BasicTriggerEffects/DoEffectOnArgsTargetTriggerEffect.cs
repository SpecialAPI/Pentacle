using Pentacle.CustomEvent.Args;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect.BasicTriggerEffects
{
    public class DoEffectOnArgsTargetTriggerEffect(TriggerEffect effect) : TriggerEffect
    {
        public TriggerEffect effect = effect;

        public override void DoEffect(IUnit sender, object args, TriggeredEffect triggerInfo, object activator = null)
        {
            if(effect == null)
                return;

            if (args is not ITargetHolder hold || hold.Target == null)
                return;

            effect.DoEffect(hold.Target, args, triggerInfo, activator);
        }
    }
}
