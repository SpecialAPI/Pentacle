using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Args.BasegameReferenceHolders
{
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
}
