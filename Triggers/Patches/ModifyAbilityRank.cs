using Pentacle.Tools;
using Pentacle.Triggers.Args;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Patches
{
    [HarmonyPatch]
    internal static class ModifyAbilityRank
    {
        private static readonly MethodInfo mar_te = AccessTools.Method(typeof(ModifyAbilityRank), nameof(ModifyAbilityRank_TriggerEvent));

        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.SetUpDefaultAbilities))]
        [HarmonyILManipulator]
        private static void ModifyAbilityRank_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CharacterCombat>($"get_{nameof(CharacterCombat.ClampedRank)}")))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, mar_te);
        }

        private static int ModifyAbilityRank_TriggerEvent(int curr, CharacterCombat cc)
        {
            var rankRef = new ModifyAbilityRankReference(curr);
            CombatManager.Instance.PostNotification(CustomTriggers.ModifyAbilityRank, cc, rankRef);

            return cc.Character.ClampRank(rankRef.abilityRank);
        }
    }
}
