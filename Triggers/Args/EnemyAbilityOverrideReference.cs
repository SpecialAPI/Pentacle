using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Args
{
    /// <summary>
    /// Used to override the abilities rolled by an enemy.
    /// <para>Sent as args by CustomTriggers.OverrideEnemyAbilityUsage.</para>
    /// </summary>
    public class EnemyAbilityOverrideReference
    {
        /// <summary>
        /// The abilities rolled by the enemy will be replaced by abilties with the indexes in this list. -1 (or any other invalid ability index) can be added to this list to make the enemy not roll any abilities at all (as long as no valid ability indexes are in the list).
        /// </summary>
        public readonly List<int> overrideAbiltyIDs = [];
    }
}
