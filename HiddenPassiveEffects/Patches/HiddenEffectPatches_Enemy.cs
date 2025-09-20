using Pentacle.Advanced;
using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.HiddenPassiveEffects.Patches
{
    [HarmonyPatch]
    internal static class HiddenEffectPatches_Enemy
    {
        private static readonly MethodInfo aache_u_a = AccessTools.Method(typeof(HiddenEffectPatches_Enemy), nameof(AttachAndConnectHiddenEffects_Unbox_Attach));
        private static readonly MethodInfo dadhe_b_d = AccessTools.Method(typeof(HiddenEffectPatches_Enemy), nameof(DettachAndDisconnectHiddenEffects_Box_Dettach));
        private static readonly MethodInfo che_i_c = AccessTools.Method(typeof(HiddenEffectPatches_Enemy), nameof(ConnectHiddenEffects_Initialization_Connect));
        private static readonly MethodInfo che_ne_c = AccessTools.Method(typeof(HiddenEffectPatches_Enemy), nameof(ConnectHiddenEffects_NewEnemy_Connect));
        private static readonly MethodInfo che_t_c = AccessTools.Method(typeof(HiddenEffectPatches_Enemy), nameof(ConnectHiddenEffects_Transform_Connect));

        private static void AttachHiddenEffects(EnemyCombat cc)
        {
            if (cc == null || cc.Enemy == null)
                return;

            if (cc.Enemy is not AdvancedEnemySO adv || adv.hiddenEffects == null)
                return;

            foreach (var e in adv.hiddenEffects)
            {
                if (e == null)
                    continue;

                e.OnTriggerAttached(cc);
            }
        }

        private static void DettachHiddenEffects(EnemyCombat cc)
        {
            if (cc == null || cc.Enemy == null)
                return;

            if (cc.Enemy is not AdvancedEnemySO adv || adv.hiddenEffects == null)
                return;

            foreach (var e in adv.hiddenEffects)
            {
                if (e == null)
                    continue;

                e.OnTriggerDettached(cc);
            }
        }

        private static void ConnectHiddenEffects(EnemyCombat cc)
        {
            if (cc == null || cc.Enemy == null)
                return;

            if (cc.Enemy is not AdvancedEnemySO adv || adv.hiddenEffects == null)
                return;

            foreach (var e in adv.hiddenEffects)
            {
                if (e == null)
                    continue;

                e.OnConnected(cc);
            }
        }

        private static void DisconnectHiddenEffects(EnemyCombat cc)
        {
            if (cc == null || cc.Enemy == null)
                return;

            if (cc.Enemy is not AdvancedEnemySO adv || adv.hiddenEffects == null)
                return;

            foreach (var e in adv.hiddenEffects)
            {
                if (e == null)
                    continue;

                e.OnDisconnected(cc);
            }
        }

        [HarmonyPatch(typeof(EnemyCombat), MethodType.Constructor, typeof(int), typeof(int), typeof(EnemySO), typeof(bool), typeof(int))]
        [HarmonyPostfix]
        private static void AttachHiddenEffects_Constructor_Postfix(EnemyCombat __instance)
        {
            AttachHiddenEffects(__instance);
        }

        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.TransformEnemy))]
        [HarmonyPostfix]
        private static void AttachHiddenEffects_Transform_Postfix(EnemyCombat __instance)
        {
            AttachHiddenEffects(__instance);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.TryUnboxEnemy))]
        [HarmonyILManipulator]
        private static void AttachAndConnectHiddenEffects_Unbox_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<EnemyCombat>(nameof(EnemyCombat.ConnectPassives))))
                return;

            crs.Emit(OpCodes.Ldloc_3);
            crs.Emit(OpCodes.Call, aache_u_a);
        }

        private static void AttachAndConnectHiddenEffects_Unbox_Attach(EnemyCombat ec)
        {
            AttachHiddenEffects(ec);
            ConnectHiddenEffects(ec);
        }

        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.FinalizationEnd))]
        [HarmonyPrefix]
        private static void DettachAndDisconnectHiddenEffects_Finalization_Prefix(EnemyCombat __instance)
        {
            DettachHiddenEffects(__instance);
            DisconnectHiddenEffects(__instance);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.TryBoxEnemy))]
        [HarmonyILManipulator]
        private static void DetachAndDisconnectHiddenEffects_Box_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<EnemyCombat>(nameof(EnemyCombat.RemoveAndDisconnectAllPassiveAbilities))))
                return;

            crs.Emit(OpCodes.Ldloc_0);
            crs.Emit(OpCodes.Call, dadhe_b_d);
        }

        private static void DettachAndDisconnectHiddenEffects_Box_Dettach(EnemyCombat ec)
        {
            DettachHiddenEffects(ec);
            DisconnectHiddenEffects(ec);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.Initialization))]
        [HarmonyILManipulator]
        private static void ConnectHiddenEffects_Initialization_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<EnemyCombat>(nameof(EnemyCombat.ConnectPassives))))
                return;

            crs.Emit(OpCodes.Ldloc, 9);
            crs.Emit(OpCodes.Call, che_i_c);
        }

        private static void ConnectHiddenEffects_Initialization_Connect(Dictionary<int, EnemyCombat>.ValueCollection.Enumerator e)
        {
            ConnectHiddenEffects(e.Current);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.AddNewEnemy))]
        [HarmonyILManipulator]
        private static void ConnectHiddenEffects_NewEnemy_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<EnemyCombat>(nameof(EnemyCombat.ConnectPassives))))
                return;

            crs.Emit(OpCodes.Ldloc_2);
            crs.Emit(OpCodes.Call, che_ne_c);
        }

        private static void ConnectHiddenEffects_NewEnemy_Connect(EnemyCombat ec)
        {
            ConnectHiddenEffects(ec);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.TryTransformEnemy))]
        [HarmonyILManipulator]
        private static void ConnectHiddenEffects_Transform_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<EnemyCombat>(nameof(EnemyCombat.ConnectPassives))))
                return;

            crs.Emit(OpCodes.Ldloc_0);
            crs.Emit(OpCodes.Call, che_t_c);
        }

        private static void ConnectHiddenEffects_Transform_Connect(EnemyCombat ec)
        {
            ConnectHiddenEffects(ec);
        }
    }
}
