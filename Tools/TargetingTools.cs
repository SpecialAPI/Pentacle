using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    /// <summary>
    /// Static class that provides tools and extension methods for TargetSlotInfo and BaseCombatTargettingSO
    /// </summary>
    public static class TargetingTools
    {
        /// <summary>
        /// Returns a target slot's "target slot offset" - the difference between the SlotID of the target slot and the SlotID of the unit in the target slot.
        /// <para>If the target slot doesn't have a unit, this method returns 0. If areTargetsSlots is false, this method always returns -1.</para>
        /// </summary>
        /// <param name="target">The target slot to get the target slot offset from.</param>
        /// <param name="areTargetsSlots">If false, this method will always return -1. This argument should be false if the targeting specifically targets units, and not slots.</param>
        /// <returns>The target slot's target slot offset.</returns>
        public static int TargetOffset(this TargetSlotInfo target, bool areTargetsSlots = true)
        {
            if (!areTargetsSlots)
                return -1;

            if (!target.HasUnit)
                return 0;

            return target.SlotID - target.Unit.SlotID;
        }
    }
}
