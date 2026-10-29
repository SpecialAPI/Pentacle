using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Patches
{
    [HarmonyPatch]
    internal static class OnPlayerTurnStartUniversal
    {
        [HarmonyPatch(typeof(SlotsCombat), nameof(SlotsCombat.CharacterSlotTurnStart))]
        [HarmonyPrefix]
        private static void OnPlayerTurnStartUniversal_Postfix()
        {
            var stats = CombatManager.Instance._stats;

            foreach(var s in stats.combatSlots.CharacterSlots)
                CombatManager.Instance.PostNotification(CustomTriggers.OnPlayerTurnStartUniversal, s, null);

            foreach (var s in stats.combatSlots.EnemySlots)
                CombatManager.Instance.PostNotification(CustomTriggers.OnPlayerTurnStartUniversal, s, null);

            foreach (var u in stats.UnitsOnField())
                CombatManager.Instance.PostNotification(CustomTriggers.OnPlayerTurnStartUniversal, u, null);
        }
    }
}
