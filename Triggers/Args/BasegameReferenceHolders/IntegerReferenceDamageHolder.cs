using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Args.BasegameReferenceHolders
{
    public class IntegerReferenceDamageHolder(IntegerReference_Damage intRef) : IIntHolder, IBoolHolder, IUnitHolder, IStringHolder
    {
        public IntegerReference_Damage intRef = intRef;

        int IIntHolder.this[int index]
        {
            get => index switch
            {
                0 => intRef.value,
                1 => intRef.affectedStartSlot,
                2 => intRef.affectedEndSlot,

                _ => 0
            };
            set
            {
                if(index == 0)
                    intRef.value = value;
                else if(index == 1)
                    PentacleLogger.LogWarning($"IntegerReferenceDamageHolder's second int value is read-only.");
                else if(index == 2)
                    PentacleLogger.LogWarning($"IntegerReferenceDamageHolder's third int value is read-only.");
            }
        }
        int IIntHolder.Value
        {
            get => intRef.value;
            set => intRef.value = value;
        }

        IUnit IUnitHolder.this[int index]
        {
            get => index switch
            {
                0 => intRef.possibleSourceUnit,
                1 => intRef.damagedUnit,

                _ => null
            };
            set => PentacleLogger.LogWarning($"IntegerReferenceDamageHolder's Unit values are read-only.");
        }
        IUnit IUnitHolder.Value
        {
            get => intRef.damagedUnit;
            set => PentacleLogger.LogWarning($"IntegerReferenceDamageHolder's Unit values are read-only.");
        }

        bool IBoolHolder.this[int index]
        {
            get => index switch
            {
                0 => intRef.directDamage,
                1 => intRef.ignoreShield,

                _ => false
            };
            set => PentacleLogger.LogWarning($"IntegerReferenceDamageHolder's bool values are read-only.");
        }
        bool IBoolHolder.Value
        {
            get => intRef.directDamage;
            set => PentacleLogger.LogWarning($"IntegerReferenceDamageHolder's bool values are read-only.");
        }

        string IStringHolder.this[int index]
        {
            get => index switch
            {
                0 => intRef.damageTypeID,
                1 => intRef.deathTypeID,

                _ => string.Empty
            };
            set => PentacleLogger.LogWarning($"IntegerReferenceHealHolder's string values are read-only.");
        }
        string IStringHolder.Value
        {
            get => intRef.damageTypeID;
            set => PentacleLogger.LogWarning($"IntegerReferenceHealHolder's string values are read-only.");
        }
    }
}
