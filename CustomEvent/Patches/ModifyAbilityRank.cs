using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Patches
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
            var intRef = new IntegerReference(curr);
            CombatManager.Instance.PostNotification(CustomTriggers.ModifyAbilityRank, cc, intRef);

            return cc.Character.ClampRank(intRef.value);
        }
    }
}
