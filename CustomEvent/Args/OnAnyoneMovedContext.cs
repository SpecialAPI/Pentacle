using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Args
{
    public class OnAnyoneMovedContext(int oldSlot, IUnit movedUnit) : IIntHolder, IUnitHolder
    {
        public int oldSlot = oldSlot;
        public IUnit movedUnit = movedUnit;

        int IIntHolder.Value
        {
            get => oldSlot;
            set => oldSlot = value;
        }
        int IIntHolder.this[int index]
        {
            get => oldSlot;
            set => oldSlot = value;
        }

        IUnit IUnitHolder.Value
        {
            get => movedUnit;
            set => movedUnit = value;
        }
        IUnit IUnitHolder.this[int index]
        {
            get => movedUnit;
            set => movedUnit = value;
        }
    }
}
