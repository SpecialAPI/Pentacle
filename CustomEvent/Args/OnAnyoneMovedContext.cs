using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Args
{
    /// <summary>
    /// Provides information about a moved unit.
    /// </summary>
    /// <param name="oldSlot"></param>
    /// <param name="movedUnit"></param>
    public class OnAnyoneMovedContext(int oldSlot, IUnit movedUnit) : IIntHolder, IUnitHolder
    {
        /// <summary>
        /// The unit's SlotID before they were moved.
        /// </summary>
        public int oldSlot = oldSlot;
        /// <summary>
        /// The unit that was moved.
        /// </summary>
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
