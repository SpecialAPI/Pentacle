using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil.Cil;
using Pentacle.Misc;

namespace Pentacle.Internal
{
    internal static class DamageReversePatches
    {
        internal static bool ReversePatchesDone;

        private static void SpecialDamage_Transpiler(ILContext ctx, MethodBase mthd)
        {
            const int SInfoArg = 9;

            if (ctx == null || mthd == null)
                return;

            var crs = new ILCursor(ctx);
            var isCharacter = mthd.DeclaringType == typeof(CharacterCombat);

            #region DisableOnBeingDamagedCalls
            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<CombatManager>($"get_{nameof(CombatManager.Instance)}")))
                return;

            if (!crs.TryFindNext(out var postNotif, x => x.MatchCallOrCallvirt<CombatManager>(nameof(CombatManager.PostNotification))))
                return;

            var afterPostNotifInstr = postNotif[0].Next.Next;

            crs.Emit(OpCodes.Ldarg, SInfoArg);
            crs.EmitStaticDelegate(SpecialDamage_DisableOnBeingDamagedCall);
            crs.Emit(OpCodes.Brtrue, afterPostNotifInstr);
            #endregion

            #region FakeDamage
            if (!crs.JumpToNext(x => x.OpCode == OpCodes.Brfalse || x.OpCode == OpCodes.Brfalse_S))
                return;

            if (!crs.TryFindNext(out var setHealth, isCharacter
                ? (Instruction x) => x.MatchCallOrCallvirt<CharacterCombat>($"set_{nameof(CharacterCombat.CurrentHealth)}")
                : (Instruction x) => x.MatchCallOrCallvirt<EnemyCombat>($"set_{nameof(EnemyCombat.CurrentHealth)}")))
                return;

            var afterSetHealth = setHealth[0].Next.Next;

            crs.Emit(OpCodes.Ldarg, SInfoArg);
            crs.EmitStaticDelegate(SpecialDamage_FakeDamage);
            crs.Emit(OpCodes.Brtrue, afterSetHealth);
            #endregion

            #region ProduceSpecialPigment
            if (!crs.JumpToNext(isCharacter
                ? (Instruction x) => x.MatchCallOrCallvirt<CharacterCombat>($"get_{nameof(CharacterCombat.HealthColor)}")
                : (Instruction x) => x.MatchCallOrCallvirt<EnemyCombat>($"get_{nameof(EnemyCombat.HealthColor)}")))
                return;

            crs.Emit(OpCodes.Ldarg, SInfoArg);
            crs.EmitStaticDelegate(SpecialDamage_ModifyPigmentColor);
            #endregion

            #region ExtraPigment
            if (!crs.JumpToNext(isCharacter
                ? (Instruction x) => x.MatchCallOrCallvirt<CombatSettings_Data>($"get_{nameof(CombatSettings_Data.CharacterPigmentAmount)}")
                : (Instruction x) => x.MatchCallOrCallvirt<CombatSettings_Data>($"get_{nameof(CombatSettings_Data.EnemyPigmentAmount)}")))
                return;

            crs.Emit(OpCodes.Ldarg, SInfoArg);
            crs.EmitStaticDelegate(SpecialDamage_ModifyPigmentAmount);
            #endregion

            #region ForcePigmentProduction
            if (!crs.JumpBeforeNext(x => x.MatchLdcI4(0)))
                return;

            crs.Emit(OpCodes.Ldarg, SInfoArg);
            crs.EmitStaticDelegate(SpecialDamage_ModifyAddPigmentAction);
            #endregion

            #region DisableOnDamageCalls
            // Need to figure out why MoveType.AfterLabels doesn't work
            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CombatManager>($"get_{nameof(CombatManager.Instance)}")))
                return;

            var brCrs = new ILCursor(crs);
            if (!brCrs.JumpBeforeNext(x => x.OpCode == OpCodes.Br || x.OpCode == OpCodes.Br_S, 2))
                return;

            var brInstr = brCrs.Next;

            crs.Emit(OpCodes.Ldarg, SInfoArg);
            crs.EmitStaticDelegate(SpecialDamage_DisableOnDamageCall);
            crs.Emit(OpCodes.Brtrue, brInstr);
            crs.EmitStaticDelegate(SpecialDamage_CombatManager);
            #endregion
        }

        private static bool SpecialDamage_FakeDamage(SpecialDamageInfo info)
        {
            if(info == null)
                return false;

            return info.FakeDamage;
        }

        private static bool SpecialDamage_DisableOnDamageCall(CombatManager _, SpecialDamageInfo info)
        {
            if(info == null)
                return false;

            return info.DisableOnDamageCalls;
        }

        private static CombatManager SpecialDamage_CombatManager()
        {
            return CombatManager.Instance;
        }

        private static bool SpecialDamage_DisableOnBeingDamagedCall(SpecialDamageInfo info)
        {
            if(info == null)
                return false;

            return info.DisableOnBeingDamagedCalls;
        }

        private static ManaColorSO SpecialDamage_ModifyPigmentColor(ManaColorSO current, SpecialDamageInfo info)
        {
            if(info == null)
                return current;

            if(!info.ProduceSpecialPigment)
                return current;

            return info.SpecialPigment;
        }

        private static IImmediateAction SpecialDamage_ModifyAddPigmentAction(AddManaToManaBarAction curr, SpecialDamageInfo info)
        {
            if (info == null)
                return curr;

            if(!info.ForcePigmentProduction)
                return curr;

            return new ForceAddPigmentAction(curr._mana, curr._amount, curr._isGeneratorCharacter, curr._id);
        }

        private static int SpecialDamage_ModifyPigmentAmount(int curr, SpecialDamageInfo info)
        {
            if (info == null)
                return curr;

            if (info.SetsPigment)
                return info.ExtraPigment;

            return curr + info.ExtraPigment;
        }

        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.Damage))]
        [HarmonyReversePatch(HarmonyReversePatchType.Snapshot)]
        internal static DamageInfo SpecialDamage_Characters_ReversePatch(CharacterCombat __instance, int amount, IUnit killer, string deathTypeID, int targetSlotOffset, bool addHealthMana, bool directDamage, bool ignoresShield, string specialDamage, SpecialDamageInfo sinfo)
        {
            static void ILManipulator(ILContext ctx, MethodBase mthd)
            {
                SpecialDamage_Transpiler(ctx, mthd);
            }

            ILManipulator(null, null);
            return default;
        }

        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.Damage))]
        [HarmonyReversePatch(HarmonyReversePatchType.Snapshot)]
        internal static DamageInfo SpecialDamage_Enemies_ReversePatch(EnemyCombat __instance, int amount, IUnit killer, string deathTypeID, int targetSlotOffset, bool addHealthMana, bool directDamage, bool ignoresShield, string specialDamage, SpecialDamageInfo sinfo)
        {
            static void ILManipulator(ILContext ctx, MethodBase mthd)
            {
                SpecialDamage_Transpiler(ctx, mthd);
            }

            ILManipulator(null, null);
            return default;
        }

        internal static void Patch()
        {
            HarmonyInstance.PatchAll(typeof(DamageReversePatches));

            ReversePatchesDone = true;
        }
    }
}
