using Pentacle.Advanced;
using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.HiddenPassiveEffects.Patches
{
    [HarmonyPatch]
    internal static class HiddenPassiveEffectPatches_Character
    {
        private static readonly MethodInfo aache_u_a = AccessTools.Method(typeof(HiddenPassiveEffectPatches_Character), nameof(AttachAndConnectHiddenEffects_Unbox_Attach));
        private static readonly MethodInfo dadhe_b_d = AccessTools.Method(typeof(HiddenPassiveEffectPatches_Character), nameof(DettachAndDisconnectHiddenEffects_Box_Dettach));
        private static readonly MethodInfo dadhe_f_d = AccessTools.Method(typeof(HiddenPassiveEffectPatches_Character), nameof(DettachAndDisconnectHiddenEffects_Fleeting_Dettach));
        private static readonly MethodInfo che_i_c = AccessTools.Method(typeof(HiddenPassiveEffectPatches_Character), nameof(ConnectHiddenEffects_Initialization_Connect));
        private static readonly MethodInfo che_r_c = AccessTools.Method(typeof(HiddenPassiveEffectPatches_Character), nameof(ConnectHiddenEffects_Resurrect_Connect));
        private static readonly MethodInfo che_nc_c = AccessTools.Method(typeof(HiddenPassiveEffectPatches_Character), nameof(ConnectHiddenEffects_NewCharacter_Connect));
        private static readonly MethodInfo che_t_c = AccessTools.Method(typeof(HiddenPassiveEffectPatches_Character), nameof(ConnectHiddenEffects_Transform_Connect));
        private static readonly MethodInfo dhe_d_d = AccessTools.Method(typeof(HiddenPassiveEffectPatches_Character), nameof(DisconnectHiddenEffects_Death_Disconnect));

        private static void AttachHiddenEffects(CharacterCombat cc)
        {
            if (cc == null || cc.Character == null)
                return;

            if (cc.Character is not AdvancedCharacterSO adv || adv.hiddenEffects == null)
                return;

            foreach (var e in adv.hiddenEffects)
            {
                if (e == null)
                    continue;

                e.OnTriggerAttached(cc);
            }
        }

        private static void DettachHiddenEffects(CharacterCombat cc)
        {
            if (cc == null || cc.Character == null)
                return;

            if (cc.Character is not AdvancedCharacterSO adv || adv.hiddenEffects == null)
                return;

            foreach (var e in adv.hiddenEffects)
            {
                if (e == null)
                    continue;

                e.OnTriggerDettached(cc);
            }
        }

        private static void ConnectHiddenEffects(CharacterCombat cc)
        {
            if (cc == null || cc.Character == null)
                return;

            if (cc.Character is not AdvancedCharacterSO adv || adv.hiddenEffects == null)
                return;

            foreach (var e in adv.hiddenEffects)
            {
                if (e == null)
                    continue;

                e.OnConnected(cc);
            }
        }

        private static void DisconnectHiddenEffects(CharacterCombat cc)
        {
            if (cc == null || cc.Character == null)
                return;

            if (cc.Character is not AdvancedCharacterSO adv || adv.hiddenEffects == null)
                return;

            foreach (var e in adv.hiddenEffects)
            {
                if (e == null)
                    continue;

                e.OnDisconnected(cc);
            }
        }

        [HarmonyPatch(typeof(CharacterCombat), MethodType.Constructor, typeof(int), typeof(int), typeof(CharacterSO), typeof(bool), typeof(int), typeof(int[]), typeof(int), typeof(BaseWearableSO), typeof(WearableStaticModifiers), typeof(bool), typeof(string), typeof(bool))]
        [HarmonyPostfix]
        private static void AttachHiddenEffects_Constructor_Postfix(CharacterCombat __instance)
        {
            AttachHiddenEffects(__instance);
        }

        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.TransformCharacter))]
        [HarmonyPostfix]
        private static void AttachHiddenEffects_Transform_Postfix(CharacterCombat __instance)
        {
            AttachHiddenEffects(__instance);
        }

        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.ResurrectCharacterStart))]
        [HarmonyPostfix]
        private static void AttachHiddenEffects_Resurrect_Postfix(CharacterCombat __instance)
        {
            AttachHiddenEffects(__instance);
        }

        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.ReConnectResurrectedCharacter))] // this one doesn't seem to be used anywhere
        [HarmonyPostfix]
        private static void AttachAndConnectHiddenEffects_Resurrect2_Postfix(CharacterCombat __instance)
        {
            AttachHiddenEffects(__instance);
            ConnectHiddenEffects(__instance);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.TryUnboxCharacter))]
        [HarmonyILManipulator]
        private static void AttachAndConnectHiddenEffects_Unbox_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.ConnectPassives))))
                return;

            crs.Emit(OpCodes.Ldloc_2);
            crs.Emit(OpCodes.Call, aache_u_a);
        }

        private static void AttachAndConnectHiddenEffects_Unbox_Attach(CharacterCombat cc)
        {
            AttachHiddenEffects(cc);
            ConnectHiddenEffects(cc);
        }

        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.TransformCharacter))]
        [HarmonyPrefix]
        private static void DettachAndDisconnectHiddenEffects_Transform_Prefix(CharacterCombat __instance)
        {
            DisconnectHiddenEffects(__instance);
            DettachHiddenEffects(__instance);
        }

        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.FinalizeDeadConnections))]
        [HarmonyPrefix]
        private static void DettachHiddenEffects_Death_Prefix(CharacterCombat __instance)
        {
            DettachHiddenEffects(__instance);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.TryBoxCharacter))]
        [HarmonyILManipulator]
        private static void DettachAndDisconnectHiddenEffects_Box_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.RemoveAndDisconnectAllPassiveAbilities))))
                return;

            crs.Emit(OpCodes.Ldloc_0);
            crs.Emit(OpCodes.Call, dadhe_b_d);
        }

        private static void DettachAndDisconnectHiddenEffects_Box_Dettach(CharacterCombat cc)
        {
            DisconnectHiddenEffects(cc);
            DettachHiddenEffects(cc);
        }

        [HarmonyPatch(typeof(FleetingUnitAction), nameof(FleetingUnitAction.Execute), MethodType.Enumerator)]
        [HarmonyILManipulator]
        private static void DettachAndDisconnectHiddenEffects_Fleeting_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.RemoveAndDisconnectAllPassiveAbilities))))
                return;

            crs.Emit(OpCodes.Ldloc_2);
            crs.Emit(OpCodes.Call, dadhe_f_d);
        }

        private static void DettachAndDisconnectHiddenEffects_Fleeting_Dettach(CharacterCombat cc)
        {
            DisconnectHiddenEffects(cc);
            DettachHiddenEffects(cc);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.Initialization))]
        [HarmonyILManipulator]
        private static void ConnectHiddenEffects_Initialization_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.ConnectPassives))))
                return;

            crs.Emit(OpCodes.Ldloc, 8);
            crs.Emit(OpCodes.Call, che_i_c);
        }

        private static void ConnectHiddenEffects_Initialization_Connect(Dictionary<int, CharacterCombat>.ValueCollection.Enumerator e)
        {
            ConnectHiddenEffects(e.Current);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.ResurrectDeadCharacter))]
        [HarmonyILManipulator]
        private static void ConnectHiddenEffects_Resurrect_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.ConnectPassives))))
                return;

            crs.Emit(OpCodes.Ldarg_1);
            crs.Emit(OpCodes.Call, che_r_c);
        }

        private static void ConnectHiddenEffects_Resurrect_Connect(CharacterCombat cc)
        {
            ConnectHiddenEffects(cc);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.AddNewCharacter))]
        [HarmonyILManipulator]
        private static void ConnectHiddenEffects_NewCharacter_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.ConnectPassives))))
                return;

            crs.Emit(OpCodes.Ldloc_2);
            crs.Emit(OpCodes.Call, che_nc_c);
        }

        private static void ConnectHiddenEffects_NewCharacter_Connect(CharacterCombat cc)
        {
            ConnectHiddenEffects(cc);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.TryTransformCharacter))]
        [HarmonyILManipulator]
        private static void ConnectHiddenEffects_Transform_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.ConnectPassives))))
                return;

            crs.Emit(OpCodes.Ldloc_0);
            crs.Emit(OpCodes.Call, che_t_c);
        }

        private static void ConnectHiddenEffects_Transform_Connect(CharacterCombat cc)
        {
            ConnectHiddenEffects(cc);
        }

        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.CharacterDeath))]
        [HarmonyILManipulator]
        private static void DisconnectHiddenEffects_Death_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.DisconnectPassives))))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, dhe_d_d);
        }

        private static void DisconnectHiddenEffects_Death_Disconnect(CharacterCombat cc)
        {
            DisconnectHiddenEffects(cc);
        }
    }
}
