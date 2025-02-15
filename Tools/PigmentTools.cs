using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    public static class PigmentTools
    {
        public static bool PigmentMatch(this ManaColorSO a, ManaColorSO b, PigmentMatchType match)
        {
            return match switch
            {
                PigmentMatchType.Exact => a.pigmentID == b.pigmentID,

                PigmentMatchType.ShareColor => a.SharesPigmentColor(b),
                PigmentMatchType.NonWrongPigment => !a.DealsCostDamage(b),

                _ => false
            };
        }
    }

    public enum PigmentMatchType
    {
        Exact,
        ShareColor,
        NonWrongPigment
    }
}
