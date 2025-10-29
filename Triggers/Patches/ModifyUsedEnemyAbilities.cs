using Pentacle.Triggers.Args;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pentacle.Triggers.Patches
{
    [HarmonyPatch]
    internal static class ModifyUsedEnemyAbilities
    {
        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.GetNextAbilitySlotUsage))]
        [HarmonyPostfix]
        public static void OverrideAbilityUsage_Prefix(EnemyCombat __instance, ref int[] __result)
        {
            var modifyAbRef = new ModifyUsedEnemyAbilitiesReference([..__result]);
            CombatManager.Instance.PostNotification(CustomTriggers.ModifyUsedEnemyAbilities, __instance, modifyAbRef);

            var abIds = modifyAbRef.usedAbilityIDs;
            for(var i = 0; i < abIds.Count; i++)
            {
                var abId = abIds[i];
                if (abId >= 0 && abId < __instance.Abilities.Count)
                    continue;

                abIds.RemoveAt(i);
                i--;
            }

            __result = [..abIds];
        }
    }
}
