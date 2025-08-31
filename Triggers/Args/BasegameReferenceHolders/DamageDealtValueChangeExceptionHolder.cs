using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Args.BasegameReferenceHolders
{
    internal class DamageDealtValueChangeExceptionHolder(DamageDealtValueChangeException exception) : IValueChangeException, IUnitHolder
    {
        public DamageDealtValueChangeException exception = exception;

        IUnit IUnitHolder.this[int index]
        {
            get => index switch
            {
                0 => exception.damagedUnit,
                1 => exception.casterUnit,

                _ => null
            };
            set => PentacleLogger.LogWarning($"DamageDealtValueChangeExceptionHolder's Unit values are read-only.");
        }
        IUnit IUnitHolder.Value
        {
            get => exception.damagedUnit;
            set => PentacleLogger.LogWarning($"DamageDealtValueChangeExceptionHolder's Unit values are read-only.");
        }

        bool IValueChangeException.DamageDealt => true;

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
