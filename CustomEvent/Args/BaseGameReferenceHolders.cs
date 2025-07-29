using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Args
{
    internal class BooleanReferenceHolder(BooleanReference boolRef) : IBoolHolder
    {
        public BooleanReference boolRef = boolRef;

        bool IBoolHolder.this[int index]
        {
            get => boolRef.value;
            set => boolRef.value = value;
        }
        bool IBoolHolder.Value
        {
            get => boolRef.value;
            set => boolRef.value = value;
        }
    }

    internal class BooleanWithTriggerReferenceHolder(BooleanWithTriggerReference boolRef) : IBoolHolder
    {
        public BooleanWithTriggerReference boolRef = boolRef;

        bool IBoolHolder.this[int index]
        {
            get => boolRef.value;
            set => boolRef.value = value;
        }
        bool IBoolHolder.Value
        {
            get => boolRef.value;
            set => boolRef.value = value;
        }
    }

    internal class IntegerReferenceHolder(IntegerReference intRef) : IIntHolder
    {
        public IntegerReference intRef = intRef;

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
    }

    internal class StringReferenceHolder(StringReference stringRef) : IStringHolder
    {
        public StringReference stringRef = stringRef;

        string IStringHolder.this[int index]
        {
            get => stringRef.value;
            set => stringRef.value = value;
        }
        string IStringHolder.Value
        {
            get => stringRef.value;
            set => stringRef.value = value;
        }
    }

    internal class StatusFieldApplicationHolder(StatusFieldApplication statusFieldApplication) : IBoolHolder, IIntHolder, IStringHolder
    {
        public StatusFieldApplication statusFieldApplication = statusFieldApplication;

        bool IBoolHolder.this[int index]
        {
            get => index switch
            {
                0 => statusFieldApplication.canBeApplied,
                1 => statusFieldApplication.isStatusPositive,

                _ => false
            };
            set
            {
                if(index == 0)
                    statusFieldApplication.canBeApplied = value;
                else if(index == 1)
                    statusFieldApplication.isStatusPositive = value;
            }
        }
        bool IBoolHolder.Value
        {
            get => statusFieldApplication.canBeApplied;
            set => statusFieldApplication.canBeApplied = value;
        }

        int IIntHolder.this[int index]
        {
            get => statusFieldApplication.amount;
            set => statusFieldApplication.amount = value;
        }
        int IIntHolder.Value
        {
            get => statusFieldApplication.amount;
            set => statusFieldApplication.amount = value;
        }

        string IStringHolder.this[int index]
        {
            get => statusFieldApplication.statusID;
            set => statusFieldApplication.statusID = value;
        }
        string IStringHolder.Value
        {
            get => statusFieldApplication.statusID;
            set => statusFieldApplication.statusID = value;
        }
    }
}
