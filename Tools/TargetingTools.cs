using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    public static class TargetingTools
    {
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
