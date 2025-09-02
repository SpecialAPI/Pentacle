using Pentacle.Triggers.Args;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pentacle.Triggers.Patches
{
    [HarmonyPatch]
    internal static class OverrideEnemyAbilityUsage
    {
        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.GetNextAbilitySlotUsage))]
        [HarmonyPrefix]
        public static bool OverrideAbilityUsage_Prefix(EnemyCombat __instance, ref int[] __result)
        {
            var overrideAbRef = new EnemyAbilityOverrideReference();
            CombatManager.Instance.PostNotification(CustomTriggers.OverrideEnemyAbilityUsage, __instance, overrideAbRef);

            var overrideAb = overrideAbRef.overrideAbiltyIDs;
            if (overrideAb == null || overrideAb.Count <= 0)
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
