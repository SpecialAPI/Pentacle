using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.UnitExtension
{
    [HarmonyPatch]
    internal static class UnitExtPatches
    {
        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.CalculateAbilityCostsDamage))]
        [HarmonyPrefix]
        private static void SetPigmentForAbility_Prefix(CharacterCombat __instance, FilledManaCost[] filledCost)
        {
            var ex = __instance.Ext();

            ex.PigmentUsedForAbility.Clear();
            ex.PigmentUsedForAbility.AddRange(filledCost.Select(x => x.Mana));
        }

        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.FinalizeAbilityActions))]
        [HarmonyPrefix]
        private static void ResetPigmentForAbility_Prefix(CharacterCombat __instance)
        {
            __instance.Ext().PigmentUsedForAbility.Clear();
        }
    }
}
