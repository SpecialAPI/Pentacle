using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Args.BasegameReferenceHolders
{
    internal class HealingReceivedValueChangeExceptionHolder(HealingReceivedValueChangeException exception) : IValueChangeException, IUnitHolder, IBoolHolder
    {
        public HealingReceivedValueChangeException exception = exception;

        IUnit IUnitHolder.this[int index]
        {
            get => index switch
            {
                0 => exception.possibleSourceUnit,
                1 => exception.healingUnit,

                _ => null
            };
            set
            {
                PentacleLogger.LogWarning($"HealingReceivedValueChangeExceptionHolder's Unit values are read-only.");
            }
        }
        IUnit IUnitHolder.Value
        {
            get => exception.healingUnit;
            set
            {
                PentacleLogger.LogWarning($"HealingReceivedValueChangeExceptionHolder's Unit values are read-only.");
            }
        }

        bool IBoolHolder.this[int index]
        {
            get => exception.directHealing;
            set => PentacleLogger.LogWarning($"HealingReceivedValueChangeExceptionHolder's bool values are read-only.");
        }
        bool IBoolHolder.Value
        {
            get => exception.directHealing;
            set => PentacleLogger.LogWarning($"HealingReceivedValueChangeExceptionHolder's bool values are read-only.");
        }

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
