using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect.BasicTriggerEffects
{
    public class StringReferenceSetterTriggerEffect(string value) : TriggerEffect
    {
        public string value = value;

        public override void DoEffect(IUnit sender, object args, TriggeredEffect triggerInfo, object activator = null)
        {
            if (!args.TryGetStringReference(out var stringRef))
                return;

            stringRef.value = value;
        }
    }
}
