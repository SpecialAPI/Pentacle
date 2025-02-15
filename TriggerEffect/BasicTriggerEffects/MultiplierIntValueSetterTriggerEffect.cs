using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect.BasicTriggerEffects
{
    public class MultiplierIntValueSetterTriggerEffect(int multiplier) : TriggerEffect
    {
        public int multiplier = multiplier;

        public override void DoEffect(IUnit sender, object args, TriggeredEffect triggerInfo, object activator = null)
        {
            if (args is DamageReceivedValueChangeException damageReceivedEx)
                damageReceivedEx.AddModifier(new MultiplyIntValueModifier(false, multiplier));

            else if (args is DamageDealtValueChangeException damageDealtEx)
                damageDealtEx.AddModifier(new MultiplyIntValueModifier(true, multiplier));

            else if (args is IntValueChangeException intEx)
                intEx.AddModifier(new MultiplyIntValueModifier(false, multiplier));
        }
    }
}
