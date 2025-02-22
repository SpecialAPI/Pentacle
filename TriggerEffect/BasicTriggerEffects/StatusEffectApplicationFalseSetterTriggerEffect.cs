using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect.BasicTriggerEffects
{
    public class StatusEffectApplicationFalseSetterTriggerEffect : TriggerEffect
    {
        public override void DoEffect(IUnit sender, object args, TriggeredEffect triggerInfo, TriggerEffectExtraInfo extraInfo)
        {
            if (args is not StatusFieldApplication application)
                return;

            application.canBeApplied = false;
        }
    }
}
