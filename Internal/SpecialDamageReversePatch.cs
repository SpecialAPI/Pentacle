using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil.Cil;
using Pentacle.Misc;

namespace Pentacle.Internal
{
    internal static class SpecialDamageReversePatch
    {
        internal static bool SpecialDamagePatchDone;

        private static readonly MethodInfo sd_mpa = AccessTools.Method(typeof(SpecialDamageReversePatch), nameof(SpecialDamage_ModifyPigmentAmount));
        private static readonly MethodInfo sd_mapa = AccessTools.Method(typeof(SpecialDamageReversePatch), nameof(SpecialDamage_ModifyAddPigmentAction));
        private static readonly MethodInfo sd_aedmp = AccessTools.Method(typeof(SpecialDamageReversePatch), nameof(SpecialDamage_ApplyExtraDamageModifierPercentage));
        private static readonly MethodInfo sd_mpc = AccessTools.Method(typeof(SpecialDamageReversePatch), nameof(SpecialDamage_ModifyPigmentColor));
        private static readonly MethodInfo sd_dobdc = AccessTools.Method(typeof(SpecialDamageReversePatch), nameof(SpecialDamage_DisableOnBeingDamagedCall));
        private static readonly MethodInfo sd_dodc = AccessTools.Method(typeof(SpecialDamageReversePatch), nameof(SpecialDamage_DisableOnDamageCall));
        private static readonly MethodInfo sd_cm = AccessTools.Method(typeof(SpecialDamageReversePatch), nameof(SpecialDamage_CombatManager));

        private static void SpecialDamage_Transpiler(ILContext ctx, MethodBase mthd)
        {
            if (ctx == null || mthd == null)
                return;

            var crs = new ILCursor(ctx);
            var processor = ctx.Body.GetILProcessor();

            var isCharacter = mthd.DeclaringType == typeof(CharacterCombat);
            var sinfoArgNumber = 3;

            for (var i = 0; i < ctx.Instrs.Count; i++)
            {
                var instr = ctx.Instrs[i];
                var opcode = OpCodes.Ldarg;

                if (!instr.MatchLdarg(out var arg))
                {
                    opcode = OpCodes.Ldarga;
                    if (!instr.MatchLdarga(out arg))
                    {
                        opcode = OpCodes.Starg;
                        if (!instr.MatchStarg(out arg))
                        {
                            continue;
                        }
                    }
                }

                if (arg < sinfoArgNumber)
                    continue;

                var newInstr = processor.Create(opcode, arg + 1);
                var thisInstr = ctx.Instrs[i];

                thisInstr.OpCode = newInstr.OpCode;
                thisInstr.Operand = newInstr.Operand;
            }

            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<CombatManager>($"get_{nameof(CombatManager.Instance)}")))
                return;

            if (!crs.TryFindNext(out var postNotif, x => x.MatchCallOrCallvirt<CombatManager>(nameof(CombatManager.PostNotification))) || postNotif == null || postNotif.Length <= 0)
                return;

            var afterPostNotifInstr = postNotif[0].Next.Next;

            crs.Emit(OpCodes.Ldarg, sinfoArgNumber);
            crs.Emit(OpCodes.Call, sd_dobdc);
            crs.Emit(OpCodes.Brtrue, afterPostNotifInstr);

            if (!crs.JumpBeforeNext(x => x.MatchStloc(3)))
                return;

            crs.Emit(OpCodes.Ldarg, sinfoArgNumber);
            crs.Emit(OpCodes.Ldarg_1);
            crs.Emit(OpCodes.Call, sd_aedmp);

            var matchHealthColor = (Func<Instruction, bool>)(isCharacter
                ? (Instruction x) => x.MatchCallOrCallvirt<CharacterCombat>($"get_{nameof(CharacterCombat.HealthColor)}")
                : (Instruction x) => x.MatchCallOrCallvirt<EnemyCombat>($"get_{nameof(EnemyCombat.HealthColor)}"));

            if (!crs.JumpToNext(matchHealthColor))
                return;

            crs.Emit(OpCodes.Ldarg, sinfoArgNumber);
            crs.Emit(OpCodes.Call, sd_mpc);

            var matchPigmentFromDamage = (Func<Instruction, bool>)(isCharacter
                ? (Instruction x) => x.MatchCallOrCallvirt<CombatSettings_Data>($"get_{nameof(CombatSettings_Data.CharacterPigmentAmount)}")
                : (Instruction x) => x.MatchCallOrCallvirt<CombatSettings_Data>($"get_{nameof(CombatSettings_Data.EnemyPigmentAmount)}"));

            if (!crs.JumpToNext(matchPigmentFromDamage))
                return;

            crs.Emit(OpCodes.Ldarg, sinfoArgNumber);
            crs.Emit(OpCodes.Call, sd_mpa);

            if (!crs.JumpBeforeNext(x => x.MatchLdcI4(0)))
                return;

            crs.Emit(OpCodes.Ldarg, sinfoArgNumber);
            crs.Emit(OpCodes.Call, sd_mapa);

            // Need to figure out why MoveType.AfterLabels doesn't work
            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CombatManager>($"get_{nameof(CombatManager.Instance)}")))
                return;

            var brCrs = new ILCursor(crs);

            if (!brCrs.JumpBeforeNext(x => x.OpCode == OpCodes.Br || x.OpCode == OpCodes.Br_S, 2))
                return;

            var brInstr = brCrs.Next;

            crs.Emit(OpCodes.Ldarg, sinfoArgNumber);
            crs.Emit(OpCodes.Call, sd_dodc);
            crs.Emit(OpCodes.Brtrue, brInstr);
            crs.Emit(OpCodes.Call, sd_cm);
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

        private static int SpecialDamage_ApplyExtraDamageModifierPercentage(int modAmount, SpecialDamageInfo info, int baseAmount)
        {
            if (info == null)
                return modAmount;

            if (info.ExtraDamageModifierPercentage == 0)
                return modAmount;

            if (info.ExtraDamageModifierPercentage == -100)
                return baseAmount;

            return (int)Mathf.LerpUnclamped(baseAmount, modAmount, 1f + (info.ExtraDamageModifierPercentage / 100f));
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
        internal static DamageInfo SpecialDamage_Characters_ReversePatch(CharacterCombat __instance, int amount, IUnit killer, SpecialDamageInfo sinfo, string deathTypeID, int targetSlotOffset, bool addHealthMana, bool directDamage, bool ignoresShield, string specialDamage)
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
        internal static DamageInfo SpecialDamage_Enemies_ReversePatch(EnemyCombat __instance, int amount, IUnit killer, SpecialDamageInfo sinfo, string deathTypeID, int targetSlotOffset, bool addHealthMana, bool directDamage, bool ignoresShield, string specialDamage)
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
            HarmonyInstance.PatchAll(typeof(SpecialDamageReversePatch));

            SpecialDamagePatchDone = true;
        }
    }
}
