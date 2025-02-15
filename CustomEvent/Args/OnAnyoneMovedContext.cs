using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Args
{
    public class OnAnyoneMovedContext(IntegerReference slotRef, IUnit movedUnit) : BasicIntReferenceHolder(slotRef), ITargetHolder
    {
        public IUnit Target => movedUnit;
    }
}
