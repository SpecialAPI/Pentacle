using Pentacle.CustomEvent.Args;
using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Patches
{
    [HarmonyPatch]
    internal static class StatusEffectOnAnyone
    {
        private static readonly MethodInfo sefa_te = AccessTools.Method(typeof(StatusEffectOnAnyone), nameof(StatusEffectFirstAplied_TriggerEvent));
        private static readonly MethodInfo sea_te = AccessTools.Method(typeof(StatusEffectOnAnyone), nameof(StatusEffectAplied_TriggerEvent));
        private static readonly MethodInfo sei_te = AccessTools.Method(typeof(StatusEffectOnAnyone), nameof(StatusEffectIncreased_TriggerEvent));

        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.ApplyStatusEffect))]
        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.ApplyStatusEffect))]
        [HarmonyILManipulator]
        private static void StatusEffectApplied_StatusEffectFirstApplied_ApplyStatusEffect_Transpiler(ILContext ctx)
        {
            var cursor = new ILCursor(ctx);

            if (!cursor.JumpToNext(x => x.MatchCallOrCallvirt<CombatManager>(nameof(CombatManager.PostNotification)), 2))
                return;

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Emit(OpCodes.Ldarg_2);
            cursor.Emit(OpCodes.Call, sefa_te);

            if (!cursor.JumpToNext(x => x.MatchCallOrCallvirt<CombatManager>(nameof(CombatManager.PostNotification))))
                return;

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Emit(OpCodes.Ldarg_2);
            cursor.Emit(OpCodes.Call, sea_te);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Emit(OpCodes.Ldarg_2);
            cursor.Emit(OpCodes.Call, sei_te);
        }

        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.IncreaseStatusEffects))]
        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.IncreaseStatusEffects))]
        [HarmonyILManipulator]
        private static void StatusEffectIncreased_IncreaseStatusEffects(ILContext ctx)
        {
            var cursor = new ILCursor(ctx);

            if (!cursor.JumpToNext(x => x.MatchCallOrCallvirt<CombatManager>(nameof(CombatManager.PostNotification))))
                return;

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldloc_2);
            cursor.Emit(OpCodes.Ldarg_1);

            cursor.Emit(OpCodes.Call, sei_te);
        }

        private static void StatusEffectFirstAplied_TriggerEvent(IUnit whoWasThisAppliedTo, StatusEffect_SO effect, int amount)
        {
            var cm = CombatManager.Instance;

            foreach (var u in cm._stats.UnitsOnField())
                cm.PostNotification(CustomTriggers.StatusEffectFirstAppliedToAnyone, u, new TargetedStatusEffectApplication(whoWasThisAppliedTo, effect, amount));
        }

        private static void StatusEffectAplied_TriggerEvent(IUnit whoWasThisAppliedTo, StatusEffect_SO effect, int amount)
        {
            var cm = CombatManager.Instance;

            foreach (var u in cm._stats.UnitsOnField())
                cm.PostNotification(CustomTriggers.StatusEffectAppliedToAnyone, u, new TargetedStatusEffectApplication(whoWasThisAppliedTo, effect, amount));
        }

        private static void StatusEffectIncreased_TriggerEvent(IUnit whoWasThisAppliedTo, StatusEffect_SO effect, int amount)
        {
            var cm = CombatManager.Instance;

            foreach (var u in cm._stats.UnitsOnField())
                cm.PostNotification(CustomTriggers.StatusEffectIncreasedOnAnyone, u, new TargetedStatusEffectApplication(whoWasThisAppliedTo, effect, amount));
        }
    }
}
