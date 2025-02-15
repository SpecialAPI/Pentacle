using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Args
{
    public class CanProducePigmentColorReference(ManaColorSO pigment, BooleanReference boolRef) : BasicBoolReferenceHolder(boolRef)
    {
        public ManaColorSO pigment = pigment;
    }
}
