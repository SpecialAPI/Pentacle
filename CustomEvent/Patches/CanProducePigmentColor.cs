using Pentacle.CustomEvent.Args;
using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Patches
{
    [HarmonyPatch]
    internal static class CanProducePigmentColor
    {
        private static readonly MethodInfo cppc_ama_te = AccessTools.Method(typeof(CanProducePigmentColor), nameof(CanProducePigmentColor_AddManaAction_TriggerEvent));
        private static readonly MethodInfo cppc_hcc_te = AccessTools.Method(typeof(CanProducePigmentColor), nameof(CanProducePigmentColor_HealthColorCondition_TriggerEvent));

        [HarmonyPatch(typeof(AddManaToManaBarAction), nameof(AddManaToManaBarAction.Execute))]
        [HarmonyILManipulator]
        private static void CanProducePigmentColor_AddManaAction_Transpiler(ILContext ctx, MethodBase mthd)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchLdfld<ManaColorSO>(nameof(ManaColorSO.canGenerateMana))))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, cppc_ama_te);
        }

        private static bool CanProducePigmentColor_AddManaAction_TriggerEvent(bool curr, AddManaToManaBarAction pigment)
        {
            return CanProducePigmentColor_HealthColorCondition_TriggerEvent(curr, pigment._mana);
        }

        [HarmonyPatch(typeof(HealthColorDetectionEffectorCondition), nameof(HealthColorDetectionEffectorCondition.MeetCondition))]
        [HarmonyILManipulator]
        private static void CanProducePigmentColor_HealthColorCondition_Transpiler(ILContext ctx, MethodBase mthd)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchLdfld<ManaColorSO>(nameof(ManaColorSO.canGenerateMana))))
                return;

            crs.Emit(OpCodes.Ldloc_0);
            crs.Emit(OpCodes.Call, cppc_hcc_te);
        }

        private static bool CanProducePigmentColor_HealthColorCondition_TriggerEvent(bool curr, ManaColorSO pigment)
        {
            if (pigment == null)
                return curr;

            var boolref = new CanProducePigmentColorReference(pigment, new BooleanReference(curr));

            foreach (var u in CombatManager.Instance._stats.UnitsOnField())
                CombatManager.Instance.PostNotification(CustomTriggers.CanProducePigmentColor, u, boolref);

            return boolref.BoolReference.value;
        }
    }
}
