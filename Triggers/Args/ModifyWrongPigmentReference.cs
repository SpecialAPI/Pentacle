using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Args
{
    /// <summary>
    /// Used to modify how much wrong pigment is used for an ability.
    /// <para>Sent as args by CustomTriggers.ModifyWrongPigmentAmount.</para>
    /// </summary>
    /// <param name="wrongPigmentAmount">The amount of wrong pigment used for the ability.</param>
    public class ModifyWrongPigmentReference(int wrongPigmentAmount) : IIntHolder
    {
        /// <summary>
        /// The amount of wrong pigment used for the ability. This value can be modified to change how much wrong pigment the ability uses.
        /// </summary>
        public int wrongPigmentAmount = wrongPigmentAmount;
        /// <summary>
        /// The unmodified amount of wrong pigment used for the ability.
        /// </summary>
        public readonly int originalWrongPigmentAmount = wrongPigmentAmount;

        int IIntHolder.Value
        {
            get => wrongPigmentAmount;
            set => wrongPigmentAmount = value;
        }
        int IIntHolder.this[int index]
        {
            get => index switch
            {
                0 => wrongPigmentAmount,
                1 => originalWrongPigmentAmount,

                _ => 0,
            };
            set
            {
                if (index == 0)
                    wrongPigmentAmount = value;
                if (index == 1)
                    PentacleLogger.LogWarning($"ModifyWrongPigmentReference's second int value is read-only.");
            }
        }
    }
}
