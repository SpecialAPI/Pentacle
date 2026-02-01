using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Args.BasegameReferenceHolders
{
    internal class HealingDealtValueChangeExceptionHolder(HealingDealtValueChangeException exception) : IValueChangeException, IUnitHolder
    {
        public HealingDealtValueChangeException exception = exception;

        IUnit IUnitHolder.this[int index]
        {
            get => index switch
            {
                0 => exception.healingUnit,
                1 => exception.casterUnit,

                _ => null
            };
            set => PentacleLogger.LogWarning($"HealingDealtValueChangeExceptionHolder's Unit values are read-only.");
        }
        IUnit IUnitHolder.Value
        {
            get => exception.healingUnit;
            set => PentacleLogger.LogWarning($"HealingDealtValueChangeExceptionHolder's Unit values are read-only.");
        }

        bool IValueChangeException.DamageDealt => true;
        int IValueChangeException.OriginalValue => exception.amount;

        void IValueChangeException.AddModifier(IntValueModifier modifier)
        {
            exception.AddModifier(modifier);
        }

        int IValueChangeException.GetModifiedValue()
        {
            return exception.GetModifiedValue();
        }
    }
}
