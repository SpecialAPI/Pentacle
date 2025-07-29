/*
using Pentacle.CustomEvent.Args;
using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect.BasicTriggerEffects
{
    public class SetForbiddenFruitCanMatchAndPassiveInformationTriggerEffect : TriggerEffect
    {
        public override void DoEffect(IUnit sender, object args, TriggeredEffect triggerInfo, TriggerEffectExtraInfo extraInfo)
        {
            if (args is not ForbiddenFruitCanMatchInfo canMatchInfo || !args.TryGetBoolReference(out var canMatchBoolRef))
                return;

            canMatchBoolRef.value = true;

            if (extraInfo.activator.TryGetActivatorNameAndSprite(out var passiveName, out var passiveSprite))
            {
                canMatchInfo.PassiveName.value = passiveName;
                canMatchInfo.PassiveSprite = passiveSprite;
            }
        }
    }
}
*/