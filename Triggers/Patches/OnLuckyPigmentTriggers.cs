using MonoMod.Cil;
using Pentacle.Tools;
using Pentacle.Triggers.Args;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Patches
{
    [HarmonyPatch]
    internal static class OnLuckyPigmentTriggers
    {
        private static readonly MethodInfo lpt_f_te = AccessTools.Method(typeof(OnLuckyPigmentTriggers), nameof(LuckyPigmentTriggers_Failure_TriggerEvent));
        private static readonly MethodInfo lpt_s_te = AccessTools.Method(typeof(OnLuckyPigmentTriggers), nameof(LuckyPigmentTriggers_Success_TriggerEvent));

        private static readonly MethodInfo randomRange_IntInt = AccessTools.Method(typeof(Random), nameof(Random.Range), [typeof(int), typeof(int)]);

        [HarmonyPatch(typeof(AddLuckyManaAction), nameof(AddLuckyManaAction.Execute), MethodType.Enumerator)]
        [HarmonyILManipulator]
        private static void LuckyPigmentTriggers_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt(randomRange_IntInt)))
                return;

            crs.Emit(OpCodes.Ldloc_2);
            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, lpt_f_te);

            if(!crs.JumpToNext(x => x.MatchCallOrCallvirt<ManaBar>(nameof(ManaBar.AddManaAmount))))
                return;

            crs.Emit(OpCodes.Ldloc_3);
            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, lpt_s_te);
        }

        private static int LuckyPigmentTriggers_Failure_TriggerEvent(int randomRoll, int percentage, IEnumerator enumerator)
        {
            if (randomRoll < percentage)
                return randomRoll;

            var stats = enumerator?.EnumeratorGetField<CombatStats>("stats");

            if (stats == null)
                return randomRoll;

            foreach(var u in stats.UnitsOnField())
                CombatManager.Instance.PostNotification(CustomTriggers.OnLuckyPigmentFailure, u, null);

            return randomRoll;
        }

        private static int LuckyPigmentTriggers_Success_TriggerEvent(int curr, int amount, IEnumerator enumerator)
        {
            var stats = enumerator?.EnumeratorGetField<CombatStats>("stats");

            if (stats == null)
                return curr;

            var luckyPigmentRef = new OnLuckyPigmentSuccessReference(amount);
            foreach(var u in stats.UnitsOnField())
                CombatManager.Instance.PostNotification(CustomTriggers.OnLuckyPigmentSuccess, u, luckyPigmentRef);

            return curr;
        }
    }
}
