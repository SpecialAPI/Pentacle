using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil.Cil;

namespace Pentacle.Internal
{
    internal static class SpecialDamageReversePatch
    {
        internal static bool SpecialDamagePatchDone;

        private static readonly MethodInfo sd_mpa = AccessTools.Method(typeof(SpecialDamageReversePatch), nameof(SpecialDamage_ModifyPigmentAmount));

        private static void SpecialDamage_Transpiler(ILContext ctx, MethodBase mthd)
        {
            if(ctx == null || mthd == null)
                return;

            var crs = new ILCursor(ctx);
            var processor = ctx.Body.GetILProcessor();

            var isCharacter = mthd.DeclaringType == typeof(CharacterCombat);
            var sinfoArgNumber = 3;

            for(var i = 0; i < ctx.Instrs.Count; i++)
            {
                var instr = ctx.Instrs[i];
                var opcode = OpCodes.Ldarg;

                if (!instr.MatchLdarg(out var arg))
                {
                    opcode = OpCodes.Ldarga;
                    if(!instr.MatchLdarga(out arg))
                    {
                        opcode = OpCodes.Starg;
                        if(!instr.MatchStarg(out arg))
                        {
                            continue;
                        }
                    }
                }

                if (arg < sinfoArgNumber)
                    continue;

                ctx.Instrs[i] = processor.Create(opcode, arg + 1);
            }

            var matchPigmentFromDamage = (Func<Instruction, bool>)(isCharacter
                ? (Instruction x) => x.MatchCallOrCallvirt<CombatSettings_Data>($"get_{nameof(CombatSettings_Data.CharacterPigmentAmount)}")
                : (Instruction x) => x.MatchCallOrCallvirt<CombatSettings_Data>($"get_{nameof(CombatSettings_Data.EnemyPigmentAmount)}"));

            if (!crs.JumpToNext(matchPigmentFromDamage))
                return;

            crs.Emit(OpCodes.Ldarg, sinfoArgNumber);
            crs.Emit(OpCodes.Call, sd_mpa);
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
