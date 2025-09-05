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

            for(var i = 0; i < modifyAbRef.usedAbilityIDs.Count; i++)
            {
                if (i >= 0 && i < __instance.Abilities.Count)
                    continue;

                modifyAbRef.usedAbilityIDs.RemoveAt(i);
                i--;
            }

            __result = [..modifyAbRef.usedAbilityIDs];
        }
    }
}
