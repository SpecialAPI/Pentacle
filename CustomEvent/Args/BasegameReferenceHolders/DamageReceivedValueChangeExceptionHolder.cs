using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Args.BasegameReferenceHolders
{
    internal class DamageReceivedValueChangeExceptionHolder(DamageReceivedValueChangeException exception) : IValueChangeException, IUnitHolder, IStringHolder, IBoolHolder
    {
        public DamageReceivedValueChangeException exception = exception;

        IUnit IUnitHolder.this[int index]
        {
            get => index switch
            {
                0 => exception.possibleSourceUnit,
                1 => exception.damagedUnit,

                _ => null
            };
            set
            {
                PentacleLogger.LogWarning($"DamageReceivedValueChangeExceptionHolder's Unit values are read-only.");
            }
        }
        IUnit IUnitHolder.Value
        {
            get => exception.possibleSourceUnit;
            set
            {
                PentacleLogger.LogWarning($"DamageReceivedValueChangeExceptionHolder's Unit values are read-only.");
            }
        }

        bool IBoolHolder.this[int index]
        {
            get => index switch
            {
                0 => exception.directDamage,
                1 => exception.ignoreShield,

                _ => false
            };
            set => PentacleLogger.LogWarning($"DamageReceivedValueChangeExceptionHolder's bool values are read-only.");
        }
        bool IBoolHolder.Value
        {
            get => exception.directDamage;
            set => PentacleLogger.LogWarning($"DamageReceivedValueChangeExceptionHolder's bool values are read-only.");
        }

        string IStringHolder.this[int index]
        {
            get => exception.damageTypeID;
            set => PentacleLogger.LogWarning($"DamageReceivedValueChangeExceptionHolder's string value is read-only.");
        }
        string IStringHolder.Value
        {
            get => exception.damageTypeID;
            set => PentacleLogger.LogWarning($"DamageReceivedValueChangeExceptionHolder's string value is read-only.");
        }

        bool IValueChangeException.DamageDealt => false;

        void IValueChangeException.AddModifier(IntValueModifier modifier)
        {
            if(modifier is ShieldIntValueModifier shield)
                exception.AddModifier(shield);
            else if(modifier is DivineProtectionIntValueModifier divineProtection)
                exception.AddModifier(divineProtection);
            else if(modifier is MaxHealthIntValueModifier maxHealth)
                exception.AddModifier(maxHealth);
            else
                exception.AddModifier(modifier);
        }

        int IValueChangeException.GetModifiedValue()
        {
            return exception.GetModifiedValue();
        }
    }
}
