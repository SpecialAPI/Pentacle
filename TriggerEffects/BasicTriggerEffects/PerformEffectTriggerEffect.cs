using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffects.BasicTriggerEffects
{
    /// <summary>
    /// A trigger effect that performs a list of effects.
    /// <para>If the TriggeredEffect containing this effect is immediate, the effects are performed as an immediate action. Otherwise, the effects are performed as a subaction.</para>
    /// </summary>
    /// <param name="effects">The effects that should be performed by this trigger effect.</param>
    public class PerformEffectTriggerEffect(List<EffectInfo> effects) : TriggerEffect
    {
        /// <summary>
        /// The effects that should be performed by this trigger effect.
        /// </summary>
        public List<EffectInfo> effects = effects;

        /// <inheritdoc/>
        public override void DoEffect(IUnit sender, object args, TriggerEffectInfo effectsAndTrigger, TriggerEffectActivationExtraInfo extraInfo)
        {
            if (effectsAndTrigger.immediate)
                CombatManager.Instance.ProcessImmediateAction(new ImmediateEffectAction([.. effects], sender, 0), true);
            else
                CombatManager.Instance.AddSubAction(new EffectAction([.. effects], sender, 0));
        }
    }
}
