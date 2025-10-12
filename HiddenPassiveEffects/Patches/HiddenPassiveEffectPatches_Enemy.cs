using Pentacle.Advanced;
using Pentacle.Tools;

namespace Pentacle.HiddenPassiveEffects.Patches
{
    [HarmonyPatch]
    internal static class HiddenPassiveEffectPatches_Enemy
    {
        private static void AddAndConnectHiddenPassiveEffects(EnemyCombat en)
        {
            if (en.Enemy is not AdvancedEnemySO advENSO || advENSO.hiddenEffects is not List<HiddenPassiveEffectSO> hiddenPassives)
                return;

            var ext = en.Ext();

            foreach (var hpe in hiddenPassives)
            {
                if (hpe == null)
                    continue;

                ext.HiddenPassiveEffects.Add(hpe);
                hpe.OnTriggerAttached(en);
            }
        }

        private static void RemoveDettachAndDisconnectHiddenPassiveEffects(EnemyCombat en)
        {
            foreach (var hpe in en.Ext().HiddenPassiveEffects)
            {
                if (hpe == null)
                    continue;

                hpe.OnTriggerDettached(en);
                hpe.OnDisconnected(en);
            }

            en.Ext().HiddenPassiveEffects.Clear();
        }

        private static void DettachAndDisconnectHiddenPassiveEffects(EnemyCombat en)
        {
            foreach (var hpe in en.Ext().HiddenPassiveEffects)
            {
                if (hpe == null)
                    continue;

                hpe.OnTriggerDettached(en);
                hpe.OnDisconnected(en);
            }
        }

        private static void DettachHiddenPassiveEffects(EnemyCombat en)
        {
            foreach (var hpe in en.Ext().HiddenPassiveEffects)
            {
                if (hpe == null)
                    continue;

                hpe.OnTriggerDettached(en);
            }
        }

        private static void DisconnectHiddenPassiveEffects(EnemyCombat en)
        {
            foreach (var hpe in en.Ext().HiddenPassiveEffects)
            {
                if (hpe == null)
                    continue;

                hpe.OnDisconnected(en);
            }
        }

        private static void AttachHiddenPassiveEffects(EnemyCombat en)
        {
            foreach (var hpe in en.Ext().HiddenPassiveEffects)
            {
                if (hpe == null)
                    continue;

                hpe.OnTriggerAttached(en);
            }
        }

        private static void ConnectHiddenPassiveEffects(EnemyCombat en)
        {
            foreach (var hpe in en.Ext().HiddenPassiveEffects)
            {
                if (hpe == null)
                    continue;

                hpe.OnConnected(en);
            }
        }

        [HarmonyPatch(typeof(EnemyCombat), MethodType.Constructor, typeof(int), typeof(int), typeof(EnemySO), typeof(bool), typeof(int))]
        [HarmonyILManipulator]
        private static void AddAndConnectHiddenPassiveEffects_Constructor_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<EnemyCombat>(nameof(EnemyCombat.DefaultPassiveAbilityInitialization))))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.EmitStaticDelegate(AddAndConnectHiddenPassiveEffects);
        }

        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.TransformEnemy))]
        [HarmonyILManipulator]
        private static void AddConnectRemoveDisconnectHiddenPassiveEffects_Transform_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<EnemyCombat>(nameof(EnemyCombat.FinalizationEnd))))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.EmitStaticDelegate(RemoveDettachAndDisconnectHiddenPassiveEffects);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<EnemyCombat>(nameof(EnemyCombat.DefaultPassiveAbilityInitialization))))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.EmitStaticDelegate(AddAndConnectHiddenPassiveEffects);
        }

        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.FinalizationEnd))]
        [HarmonyPrefix]
        private static void DettachAndDisconnectHiddenPassiveEffects_FinalizationEnd_Prefix(EnemyCombat __instance)
        {
            DettachAndDisconnectHiddenPassiveEffects(__instance);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.Initialization))]
        [HarmonyILManipulator]
        private static void ConnectHiddenPassiveEffects_CombatInitialization_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<EnemyCombat>(nameof(EnemyCombat.ConnectPassives))))
                return;

            crs.Emit(OpCodes.Ldloc, 9);
            crs.EmitStaticDelegate(ConnectHiddenPassiveEffects_CombatInitialization_Connect);
        }

        private static void ConnectHiddenPassiveEffects_CombatInitialization_Connect(Dictionary<int, EnemyCombat>.ValueCollection.Enumerator enumerator)
        {
            ConnectHiddenPassiveEffects(enumerator.Current);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.TryTransformEnemy))]
        [HarmonyILManipulator]
        private static void ConnectHiddenPassiveEffects_Transform_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<EnemyCombat>(nameof(EnemyCombat.ConnectPassives))))
                return;

            crs.Emit(OpCodes.Ldloc_0);
            crs.EmitStaticDelegate(ConnectHiddenPassiveEffects);
        }
    }
}
