using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Args.BasegameReferenceHolders
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
}
