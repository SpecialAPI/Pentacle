using Pentacle.CustomEvent.Args;
using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Patches
{
    [HarmonyPatch]
    internal static class AbilityPerformedTriggers
    {
        private static readonly MethodInfo bae_c_te = AccessTools.Method(typeof(AbilityPerformedTriggers), nameof(BeforeAbilityEffects_Character_TriggerEvent));
        private static readonly MethodInfo bae_e_te = AccessTools.Method(typeof(AbilityPerformedTriggers), nameof(BeforeAbilityEffects_Enemy_TriggerEvent));

        private static readonly MethodInfo apc_c_te = AccessTools.Method(typeof(AbilityPerformedTriggers), nameof(AbilityPerformedContext_Character_TriggerEvent));
        private static readonly MethodInfo apc_e_te = AccessTools.Method(typeof(AbilityPerformedTriggers), nameof(AbilityPerformedContext_Enemy_TriggerEvent));

        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.UseAbility), typeof(int))]
        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.UseAbility), typeof(int), typeof(FilledManaCost[]))]
        [HarmonyILManipulator]
        private static void AbilityPerformedTriggers_UseAbility_Transpiler(ILContext ctx, MethodBase mthd)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchNewobj<EffectAction>()))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Ldarg_1);
            crs.Emit(OpCodes.Ldloc_0);

            if (mthd.DeclaringType == typeof(CharacterCombat))
            {
                crs.Emit(OpCodes.Ldarg_2);
                crs.Emit(OpCodes.Call, bae_c_te);
            }

            else
                crs.Emit(OpCodes.Call, bae_e_te);

            if (!crs.JumpToNext(x => x.MatchNewobj<EndAbilityAction>()))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Ldarg_1);
            crs.Emit(OpCodes.Ldloc_0);

            if (mthd.DeclaringType == typeof(CharacterCombat))
            {
                crs.Emit(OpCodes.Ldarg_2);
                crs.Emit(OpCodes.Call, apc_c_te);
            }

            else
                crs.Emit(OpCodes.Call, apc_e_te);
        }

        private static EffectAction BeforeAbilityEffects_Character_TriggerEvent(EffectAction curr, CharacterCombat ch, int abilityIdx, AbilitySO ability, FilledManaCost[] cost)
        {
            CombatManager.Instance.AddRootAction(new AbilityContextNotifyAction(ch, CustomTriggers.OnBeforeAbilityEffects, new()
            {
                ability = ability,
                abilityIndex = abilityIdx,
                cost = cost
            }));

            return curr;
        }

        private static EffectAction BeforeAbilityEffects_Enemy_TriggerEvent(EffectAction curr, EnemyCombat en, int abilityIdx, AbilitySO ability)
        {
            CombatManager.Instance.AddRootAction(new AbilityContextNotifyAction(en, CustomTriggers.OnBeforeAbilityEffects, new()
            {
                ability = ability,
                abilityIndex = abilityIdx,
                cost = null
            }));

            return curr;
        }

        private static EndAbilityAction AbilityPerformedContext_Character_TriggerEvent(EndAbilityAction curr, CharacterCombat ch, int abilityIdx, AbilitySO ability, FilledManaCost[] cost)
        {
            CombatManager.Instance.AddRootAction(new AbilityContextNotifyAction(ch, CustomTriggers.OnAbilityPerformedContext, new()
            {
                ability = ability,
                abilityIndex = abilityIdx,
                cost = cost
            }));

            return curr;
        }

        private static EndAbilityAction AbilityPerformedContext_Enemy_TriggerEvent(EndAbilityAction curr, EnemyCombat en, int abilityIdx, AbilitySO ability)
        {
            CombatManager.Instance.AddRootAction(new AbilityContextNotifyAction(en, CustomTriggers.OnBeforeAbilityEffects, new()
            {
                ability = ability,
                abilityIndex = abilityIdx,
                cost = null
            }));

            return curr;
        }
    }

    internal class AbilityContextNotifyAction(IUnit unit, string notif, AbilityUsedContext ctx) : CombatAction
    {
        public override IEnumerator Execute(CombatStats stats)
        {
            CombatManager.Instance.PostNotification(notif, unit, ctx);

            yield break;
        }
    }
}
