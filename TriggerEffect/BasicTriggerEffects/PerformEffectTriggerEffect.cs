using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect.BasicTriggerEffects
{
    /// <summary>
    /// A Trigger Effect that triggers a given list of effects when performed.
    /// </summary>
    /// <param name="effects"></param>
    public class PerformEffectTriggerEffect(List<EffectInfo> effects) : TriggerEffect
    {
        public List<EffectInfo> effects = effects;

        public override void DoEffect(IUnit sender, object args, TriggeredEffect effectsAndTrigger, TriggerEffectExtraInfo extraInfo)
        {
            if (effectsAndTrigger.immediate)
                CombatManager.Instance.ProcessImmediateAction(new ImmediateEffectAction([.. effects], sender, 0), true);
            else
                CombatManager.Instance.AddSubAction(new EffectAction([.. effects], sender, 0));
        }
    }
}
