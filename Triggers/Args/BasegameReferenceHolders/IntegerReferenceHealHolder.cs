using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Args.BasegameReferenceHolders
{
    internal class IntegerReferenceHealHolder(IntegerReference_Heal intRef) : IIntHolder, IBoolHolder, IUnitHolder, IStringHolder
    {
        public IntegerReference_Heal intRef = intRef;

        int IIntHolder.this[int index]
        {
            get => intRef.value;
            set => intRef.value = value;
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
                1 => intRef.healedUnit,

                _ => null
            };
            set => PentacleLogger.LogWarning($"IntegerReferenceHealHolder's Unit values are read-only.");
        }
        IUnit IUnitHolder.Value
        {
            get => intRef.healedUnit;
            set => PentacleLogger.LogWarning($"IntegerReferenceHealHolder's Unit values are read-only.");
        }

        bool IBoolHolder.this[int index]
        {
            get => intRef.directHeal;
            set => PentacleLogger.LogWarning($"IntegerReferenceHealHolder's bool value is read-only.");
        }
        bool IBoolHolder.Value
        {
            get => intRef.directHeal;
            set => PentacleLogger.LogWarning($"IntegerReferenceHealHolder's bool value is read-only.");
        }

        string IStringHolder.this[int index]
        {
            get => intRef.healTypeID;
            set => PentacleLogger.LogWarning($"IntegerReferenceHealHolder's string value is read-only.");
        }
        string IStringHolder.Value
        {
            get => intRef.healTypeID;
            set => PentacleLogger.LogWarning($"IntegerReferenceHealHolder's string value is read-only.");
        }
    }
}
