using System;
using System.Collections.Generic;
using System.Text;
using Pentacle.Tools;

namespace Pentacle.CombatActions
{
    /// <summary>
    /// A UI action that modifies the visually displayed pigment in overflow. 
    /// </summary>
    /// <param name="slots">The indexes of the pigments that should be changed. This list's length should match the length of <paramref name="slots"/>.</param>
    /// <param name="pigment">The new pigment colors for the pigments in those slots. This list's length should match the length of <paramref name="pigment"/>.</param>
    public class ModifyOverflowUIAction(List<int> slots, List<ManaColorSO> pigment) : CombatAction
    {
        public override IEnumerator Execute(CombatStats stats)
        {
            yield return stats.combatUI.ModifyOverflowPigment(slots, pigment);
        }
    }
}
