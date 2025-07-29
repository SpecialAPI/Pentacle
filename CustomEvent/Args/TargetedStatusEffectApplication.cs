using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Args
{
    public class TargetedStatusEffectApplication(IUnit unit, StatusEffect_SO statusEffect, int amountToApply) : IUnitHolder
    {
        public IUnit unit = unit;
        public StatusEffect_SO statusEffect = statusEffect;
        public int amountToApply = amountToApply;

        IUnit IUnitHolder.Value
        {
            get => unit;
            set => unit = value;
        }
        IUnit IUnitHolder.this[int index]
        {
            get => unit;
            set => unit = value;
        }
    }
}
