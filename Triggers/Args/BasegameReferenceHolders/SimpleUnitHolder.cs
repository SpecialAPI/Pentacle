using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Args.BasegameReferenceHolders
{
    internal class SimpleUnitHolder(IUnit unit) : IUnitHolder
    {
        public IUnit unit = unit;

        public IUnit this[int index] { get => unit; set => PentacleLogger.LogWarning($"SimpleUnitHolder's Unit value is read-only."); }
        public IUnit Value { get => unit; set => PentacleLogger.LogWarning($"SimpleUnitHolder's Unit value is read-only."); }
    }
}
