using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Args
{
    /// <summary>
    /// Provides information about a moved unit.
    /// <para>Sent as args by CustomTriggers.OnAnyoneMoved.</para>
    /// </summary>
    /// <param name="oldSlot">The unit's SlotID before they were moved.</param>
    /// <param name="movedUnit">The unit that was moved.</param>
    public class OnAnyoneMovedContext(int oldSlot, IUnit movedUnit) : IIntHolder, IUnitHolder
    {
        /// <summary>
        /// The unit's SlotID before they were moved.
        /// </summary>
        public readonly int oldSlot = oldSlot;
        /// <summary>
        /// The unit that was moved.
        /// </summary>
        public readonly IUnit movedUnit = movedUnit;

        int IIntHolder.Value
        {
            get => oldSlot;
            set => PentacleLogger.LogWarning("OnAnyoneMovedContext's int value is read-only.");
        }
        int IIntHolder.this[int index]
        {
            get => oldSlot;
            set => PentacleLogger.LogWarning("OnAnyoneMovedContext's int value is read-only.");
        }

        IUnit IUnitHolder.Value
        {
            get => movedUnit;
            set => PentacleLogger.LogWarning("OnAnyoneMovedContext's Unit value is read-only.");
        }
        IUnit IUnitHolder.this[int index]
        {
            get => movedUnit;
            set => PentacleLogger.LogWarning("OnAnyoneMovedContext's Unit value is read-only.");
        }
    }
}
