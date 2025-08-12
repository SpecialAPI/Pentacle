using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    /// <summary>
    /// Static class that provides pigment-related tools and extensions.
    /// </summary>
    public static class PigmentTools
    {
        /// <summary>
        /// Checks if two pigment colors match based on certain criteria.
        /// </summary>
        /// <param name="a">The first pigment color.</param>
        /// <param name="b">The second pigment color.</param>
        /// <param name="match">The criteria for the two pigment colors to be considered "matching".</param>
        /// <returns>True if the pigment colors match, false otherwise.</returns>
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

    /// <summary>
    /// Types of criteria for PigmentTools.PigmentMatch to consider two pigment colors "matching".
    /// </summary>
    public enum PigmentMatchType
    {
        /// <summary>
        /// Pigment colors are considered "matching" if their pigment IDs are equal.
        /// </summary>
        Exact,
        /// <summary>
        /// Pigment colors are considered "matching" if they share a pigment type.
        /// </summary>
        ShareColor,
        /// <summary>
        /// Pigment colors are considered "matching" if using one of them to fill a cost of the other color wouldn't be considered wrong pigment.
        /// </summary>
        NonWrongPigment
    }
}
