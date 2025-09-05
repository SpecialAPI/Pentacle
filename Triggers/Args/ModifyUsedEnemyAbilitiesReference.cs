using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Args
{
    /// <summary>
    /// Used to override the abilities rolled by an enemy.
    /// <para>Sent as args by CustomTriggers.OverrideEnemyAbilityUsage.</para>
    /// </summary>
    /// <param name="rolledAbilityIDs">The indexes of the abilities the enemy will use.</param>
    public class ModifyUsedEnemyAbilitiesReference(List<int> rolledAbilityIDs)
    {
        /// <summary>
        /// The indexes of the abilities the enemy will use. Ability indexes can be added to or removed from this list to modify which abilities the enemy will use.
        /// </summary>
        public readonly List<int> usedAbilityIDs = rolledAbilityIDs;
        /// <summary>
        /// The indexes of the abilities the enemy was originally going to use.
        /// </summary>
        public readonly IReadOnlyList<int> originalUsedAbilityIDs = rolledAbilityIDs;
    }
}
