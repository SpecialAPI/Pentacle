using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pentacle.CustomEvent.Patches
{
    [HarmonyPatch]
    internal static class OverrideEnemyAbilityUsage
    {
        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.GetNextAbilitySlotUsage))]
        [HarmonyPrefix]
        public static bool OverrideAbilityUsage_Prefix(EnemyCombat __instance, ref int[] __result)
        {
            var overrideAb = new List<int>();
            CombatManager.Instance.PostNotification(CustomTriggers.OverrideEnemyAbilityUsage, __instance, overrideAb);

            if (overrideAb.Count <= 0)
                return true;

            var res = new List<int>();
            foreach (var ab in overrideAb)
            {
                if (ab < 0 || ab >= __instance.Abilities.Count)
                    continue;

                res.Add(ab);
            }
            __result = [.. res];

            return false;
        }
    }
}
