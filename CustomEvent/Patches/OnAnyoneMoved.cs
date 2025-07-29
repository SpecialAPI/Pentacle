using Pentacle.CustomEvent.Args;
using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Patches
{
    [HarmonyPatch]
    internal static class OnAnyoneMoved
    {
        private static readonly MethodInfo am_te = AccessTools.Method(typeof(OnAnyoneMoved), nameof(AnyoneMoved_TriggerEvent));
    
        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.SwappedTo))]
        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.SwappedTo))]
        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.SwapTo))]
        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.SwapTo))]
        [HarmonyILManipulator]
        private static void AnyoneMoved_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);
    
            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CombatManager>(nameof(CombatManager.PostNotification)), 2))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Ldloc_0);
            crs.Emit(OpCodes.Call, am_te);
        }
    
        private static void AnyoneMoved_TriggerEvent(IUnit unit, int oldsid)
        {
            foreach (var kvp in CombatManager.Instance._stats.UnitsOnField())
                CombatManager.Instance.PostNotification(CustomTriggers.OnAnyoneMoved, kvp, new OnAnyoneMovedContext(oldsid, unit));
        }
    }
}
