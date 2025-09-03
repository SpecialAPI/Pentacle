using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Args
{
    /// <summary>
    /// Used to modify the level of a character's abilities.
    /// <para>Sent as args by CustomTriggers.ModifyAbilityRank.</para>
    /// </summary>
    /// <param name="abilityRank">The level of the character's abilities.</param>
    public class ModifyAbilityRankReference(int abilityRank) : IIntHolder
    {
        /// <summary>
        /// The level of the character's abilities. This value can be modified to change which level the character's abilities will be.
        /// </summary>
        public int abilityRank = abilityRank;
        /// <summary>
        /// The unmodified level of the character's abilities.
        /// </summary>
        public readonly int originalAbilityRank = abilityRank;

        int IIntHolder.Value
        {
            get => abilityRank;
            set => abilityRank = value;
        }
        int IIntHolder.this[int index]
        {
            get => index switch
            {
                0 => abilityRank,
                1 => originalAbilityRank,

                _ => 0,
            };
            set
            {
                if (index == 0)
                    abilityRank = value;
                if (index == 1)
                    PentacleLogger.LogWarning($"ModifyWrongPigmentReference's second int value is read-only.");
            }
        }
    }
}
