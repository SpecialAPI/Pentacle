using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Args.BasegameReferenceHolders
{
    internal class SimpleUnitHolder(IUnit u) : IUnitHolder
    {
        public IUnit unit = u;

        public IUnit this[int index] { get => u; set => PentacleLogger.LogWarning($"SimpleUnitHolder's Unit value is read-only."); }
        public IUnit Value { get => u; set => PentacleLogger.LogWarning($"SimpleUnitHolder's Unit value is read-only."); }
    }
}
