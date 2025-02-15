using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect.BasicTriggerEffects
{
    public class BoolReferenceSetterTriggerffect(bool value) : TriggerEffect
    {
        public bool value = value;

        public override void DoEffect(IUnit sender, object args, TriggeredEffect triggerInfo, object activator = null)
        {
            if (!args.TryGetBoolReference(out var boolRef))
                return;

            boolRef.value = value;
        }
    }
}
