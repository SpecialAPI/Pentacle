using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Args
{
    /// <summary>
    /// Used as args by various custom triggers, contains the information about a performed ability.
    /// </summary>
    public class AbilityUsedContext
    {
        /// <summary>
        /// Index of the performed ability in the unit's abilities list.
        /// </summary>
        public int abilityIndex;
        /// <summary>
        /// The performed ability.
        /// </summary>
        public AbilitySO ability;
        /// <summary>
        /// The cost used for the ability. Null for enemies.
        /// </summary>
        public FilledManaCost[] cost;
    }
}
