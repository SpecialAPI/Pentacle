using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Args.BasegameReferenceHolders
{
    internal class IntValueChangeExceptionHolder(IntValueChangeException exception) : IValueChangeException
    {
        public IntValueChangeException exception = exception;

        void IValueChangeException.AddModifier(IntValueModifier modifier)
        {
            exception.AddModifier(modifier);
        }

        int IValueChangeException.GetModifiedValue()
        {
            return exception.GetModifiedValue();
        }

        bool IValueChangeException.DamageDealt => false;
        int IValueChangeException.OriginalValue => exception.amount;
    }
}
