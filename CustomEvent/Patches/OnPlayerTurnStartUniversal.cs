using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Patches
{
    [HarmonyPatch]
    internal static class OnPlayerTurnStartUniversal
    {
        private static readonly MethodInfo optsu_te = AccessTools.Method(typeof(OnPlayerTurnStartUniversal), nameof(OnPlayerTurnStartUniversal_TriggerEvent));

        [HarmonyPatch(typeof(PlayerTurnStartSecondPartAction), nameof(PlayerTurnStartSecondPartAction.Execute), MethodType.Enumerator)]
        [HarmonyILManipulator]
        private static void OnPlayerTurnStartUniversal_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<CombatStats>(nameof(CombatStats.FillPigmentGenerator))))
                return;

            crs.Emit(OpCodes.Call, optsu_te);
        }

        private static CombatStats OnPlayerTurnStartUniversal_TriggerEvent(CombatStats stats)
        {
            foreach(var s in stats.combatSlots.CharacterSlots)
                CombatManager.Instance.PostNotification(CustomTriggers.OnPlayerTurnStartUniversal, s, null);

            foreach (var s in stats.combatSlots.EnemySlots)
                CombatManager.Instance.PostNotification(CustomTriggers.OnPlayerTurnStartUniversal, s, null);

            foreach (var u in stats.UnitsOnField())
                CombatManager.Instance.PostNotification(CustomTriggers.OnPlayerTurnStartUniversal, u, null);

            return stats;
        }
    }
}
