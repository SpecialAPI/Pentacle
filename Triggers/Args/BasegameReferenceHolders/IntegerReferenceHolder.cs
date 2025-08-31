using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Args.BasegameReferenceHolders
{
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
}
