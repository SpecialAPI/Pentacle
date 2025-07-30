using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Args
{
    /// <summary>
    /// Provides information about an ability that's being performed.
    /// <para>Sent as args by CustomTriggers.OnAbilityPerformedContext and CustomTriggers.OnBeforeAbilityEffects.</para>
    /// </summary>
    public class AbilityUsedContext(int abilityIndex, AbilitySO ability, FilledManaCost[] cost = null)
    {
        /// <summary>
        /// The index of the ability that's being performed in the Abilities list of the unit performing it.
        /// </summary>
        public readonly int abilityIndex = abilityIndex;
        /// <summary>
        /// The AbilitySO object of the ability being performed.
        /// </summary>
        public readonly AbilitySO ability = ability;
        /// <summary>
        /// The pigment that was used for the ability's cost.
        /// <para>Null for enemies.</para>
        /// </summary>
        public readonly FilledManaCost[] cost = cost;
    }
}
