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
        private static void AddAndConnectHiddenPassiveEffects(CharacterCombat ch)
        {
            if (ch.Character is not AdvancedCharacterSO advCHSO || advCHSO.hiddenEffects is not List<HiddenPassiveEffectSO> hiddenPassives)
                return;

            var ext = ch.Ext();

            foreach(var hpe in hiddenPassives)
            {
                if (hpe == null)
                    continue;

                ext.HiddenPassiveEffects.Add(hpe);
                hpe.OnTriggerAttached(ch);
            }
        }

        private static void RemoveDettachAndDisconnectHiddenPassiveEffects(CharacterCombat ch)
        {
            foreach(var hpe in ch.Ext().HiddenPassiveEffects)
            {
                if(hpe == null)
                    continue;

                hpe.OnTriggerDettached(ch);
                hpe.OnDisconnected(ch);
            }

            ch.Ext().HiddenPassiveEffects.Clear();
        }

        private static void DettachAndDisconnectHiddenPassiveEffects(CharacterCombat ch)
        {
            foreach (var hpe in ch.Ext().HiddenPassiveEffects)
            {
                if (hpe == null)
                    continue;

                hpe.OnTriggerDettached(ch);
                hpe.OnDisconnected(ch);
            }
        }

        private static void DettachHiddenPassiveEffects(CharacterCombat ch)
        {
            foreach (var hpe in ch.Ext().HiddenPassiveEffects)
            {
                if (hpe == null)
                    continue;

                hpe.OnTriggerDettached(ch);
            }
        }

        private static void DisconnectHiddenPassiveEffects(CharacterCombat ch)
        {
            foreach (var hpe in ch.Ext().HiddenPassiveEffects)
            {
                if (hpe == null)
                    continue;

                hpe.OnDisconnected(ch);
            }
        }

        private static void AttachHiddenPassiveEffects(CharacterCombat ch)
        {
            foreach (var hpe in ch.Ext().HiddenPassiveEffects)
            {
                if (hpe == null)
                    continue;

                hpe.OnTriggerAttached(ch);
            }
        }

        private static void ConnectHiddenPassiveEffects(CharacterCombat ch)
        {
            foreach (var hpe in ch.Ext().HiddenPassiveEffects)
            {
                if (hpe == null)
                    continue;

                hpe.OnConnected(ch);
            }
        }

        [HarmonyPatch(typeof(CharacterCombat), MethodType.Constructor, typeof(int), typeof(int), typeof(CharacterSO), typeof(bool), typeof(int), typeof(int[]), typeof(int), typeof(BaseWearableSO), typeof(WearableStaticModifiers), typeof(bool), typeof(string), typeof(bool))]
        [HarmonyILManipulator]
        private static void AddAndConnectHiddenPassiveEffects_Constructor_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.DefaultPassiveAbilityInitialization))))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.EmitStaticDelegate(AddAndConnectHiddenPassiveEffects);
        }

        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.TransformCharacter))]
        [HarmonyILManipulator]
        private static void AddConnectRemoveDisconnectHiddenPassiveEffects_Transform_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.RemoveAndDisconnectAllPassiveAbilities))))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.EmitStaticDelegate(RemoveDettachAndDisconnectHiddenPassiveEffects);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.DefaultPassiveAbilityInitialization))))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.EmitStaticDelegate(AddAndConnectHiddenPassiveEffects);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.TryUnboxCharacter))]
        [HarmonyILManipulator]
        private static void ReconnectHiddenPassiveEffects_Unbox_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.DefaultPassiveAbilityInitialization))))
                return;

            crs.Emit(OpCodes.Ldloc_2);
            crs.EmitStaticDelegate(AttachHiddenPassiveEffects);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.ConnectPassives))))
                return;

            crs.Emit(OpCodes.Ldloc_2);
            crs.EmitStaticDelegate(ConnectHiddenPassiveEffects);
        }

        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.FinalizeDeadConnections))]
        [HarmonyILManipulator]
        private static void DettachHiddenPassiveEffects_FinalizeDeadConnections_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.DettachPassives))))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.EmitStaticDelegate(DettachHiddenPassiveEffects);
        }

        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.ResurrectCharacterStart))]
        [HarmonyILManipulator]
        private static void ReconnectHiddenPassiveEffects_ResurrectStart_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.AttachAllPassives))))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.EmitStaticDelegate(AttachHiddenPassiveEffects);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.Initialization))]
        [HarmonyILManipulator]
        private static void ConnectHiddenPassiveEffects_CombatInitialization_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.ConnectPassives))))
                return;

            crs.Emit(OpCodes.Ldloc, 8);
            crs.EmitStaticDelegate(ConnectHiddenPassiveEffects_CombatInitialization_Connect);
        }

        private static void ConnectHiddenPassiveEffects_CombatInitialization_Connect(Dictionary<int, CharacterCombat>.ValueCollection.Enumerator enumerator)
        {
            ConnectHiddenPassiveEffects(enumerator.Current);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.ResurrectDeadCharacter))]
        [HarmonyILManipulator]
        private static void ReconnectHiddenPassiveEffects_Resurrect_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.ConnectPassives))))
                return;

            crs.Emit(OpCodes.Ldarg_1);
            crs.EmitStaticDelegate(ConnectHiddenPassiveEffects);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.AddNewCharacter))]
        [HarmonyILManipulator]
        private static void ConnectHiddenPassiveEffects_NewCharacter_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.ConnectPassives))))
                return;

            crs.Emit(OpCodes.Ldloc_2);
            crs.EmitStaticDelegate(ConnectHiddenPassiveEffects);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.TryTransformCharacter))]
        [HarmonyILManipulator]
        private static void ConnectHiddenPassiveEffects_Transform_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.ConnectPassives))))
                return;

            crs.Emit(OpCodes.Ldloc_0);
            crs.EmitStaticDelegate(ConnectHiddenPassiveEffects);
        }

        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.CharacterDeath))]
        [HarmonyILManipulator]
        private static void DisconnectHiddenPassiveEffects_Death_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.DisconnectPassives))))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.EmitStaticDelegate(DisconnectHiddenPassiveEffects);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.TryBoxCharacter))]
        [HarmonyILManipulator]
        private static void DisconnectHiddenPassiveEffects_Box_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.DisconnectPassives))))
                return;

            crs.Emit(OpCodes.Ldloc_0);
            crs.EmitStaticDelegate(DisconnectHiddenPassiveEffects);

            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.RemoveAndDisconnectAllPassiveAbilities))))
                return;

            crs.Emit(OpCodes.Ldloc_0);
            crs.EmitStaticDelegate(DettachHiddenPassiveEffects);
        }

        [HarmonyPatch(typeof(FleetingUnitAction), nameof(FleetingUnitAction.Execute), MethodType.Enumerator)]
        [HarmonyILManipulator]
        private static void DisconnectHiddenPassiveEffects_Fleeting_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.DisconnectPassives))))
                return;

            crs.Emit(OpCodes.Ldloc_2);
            crs.EmitStaticDelegate(DisconnectHiddenPassiveEffects);

            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<CharacterCombat>(nameof(CharacterCombat.RemoveAndDisconnectAllPassiveAbilities))))
                return;

            crs.Emit(OpCodes.Ldloc_2);
            crs.EmitStaticDelegate(DettachHiddenPassiveEffects);
        }
    }
}
