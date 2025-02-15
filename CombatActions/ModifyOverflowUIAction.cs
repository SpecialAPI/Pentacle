using System;
using System.Collections.Generic;
using System.Text;
using Pentacle.Tools;

namespace Pentacle.CombatActions
{
    public class ModifyOverflowUIAction(List<int> slots, List<ManaColorSO> pigment) : CombatAction
    {
        public override IEnumerator Execute(CombatStats stats)
        {
            yield return stats.combatUI.ModifyOverflowPigment(slots, pigment);
        }
    }
}
