using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Args
{
    public class TargetedStatusEffectApplication(IUnit guy, StatusEffect_SO statusEffect, int amountToApply) : ITargetHolder
    {
        public IUnit unit = guy;
        public StatusEffect_SO statusEffect = statusEffect;
        public int amountToApply = amountToApply;

        public IUnit Target => unit;
    }
}
