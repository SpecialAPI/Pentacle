using Pentacle.Tools;
using Pentacle.Triggers.Args;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Triggers.Patches
{
    [HarmonyPatch]
    internal static class ModifyWrongPigmentAmount
    {
        public static MethodInfo mwpa_ui_te = AccessTools.Method(typeof(ModifyWrongPigmentAmount), nameof(ModifyWrongPigmentAmount_UI_TriggerEvent));
        public static MethodInfo mwpa_c_te = AccessTools.Method(typeof(ModifyWrongPigmentAmount), nameof(ModifyWrongPigmentAmount_Character_TriggerEvent));

        [HarmonyPatch(typeof(AttackCostLayout), nameof(AttackCostLayout.CalculatePerformAttackButtonState))]
        [HarmonyILManipulator]
        private static void ModifyWrongPigmentAmount_UI_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            foreach (var m in crs.MatchAfter(x => x.OpCode == OpCodes.Blt_S))
            {
                crs.Emit(OpCodes.Ldarg_0);
                crs.Emit(OpCodes.Call, mwpa_ui_te);
            }
        }

        private static void ModifyWrongPigmentAmount_UI_TriggerEvent(AttackCostLayout layout)
        {
            var manager = CombatManager.Instance;
            var stats = manager._stats;
            var ui = manager._combatUI;

            if (!stats.TryGetUnit(ui.UnitInInfoID, ui.IsInfoFromCharacter, out var unit))
                return;

            var intRef = new ModifyWrongPigmentReference(layout.SlotsThatDealDamage);
            CombatManager.Instance.PostNotification(CustomTriggers.ModifyWrongPigmentAmount_UI, unit, intRef);

            layout.SlotsThatDealDamage = Mathf.Max(0, intRef.wrongPigmentAmount);
        }

        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.CalculateAbilityCostsDamage))]
        [HarmonyILManipulator]
        private static void ModifyWrongPigmentAmount_Character_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<CharacterCombat>($"get_{nameof(CharacterCombat.LastCalculatedWrongMana)}"), 2))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, mwpa_c_te);
        }

        private static void ModifyWrongPigmentAmount_Character_TriggerEvent(CharacterCombat cc)
        {
            var intRef = new ModifyWrongPigmentReference(cc.LastCalculatedWrongMana);
            CombatManager.Instance.PostNotification(CustomTriggers.ModifyWrongPigmentAmount, cc, intRef);

            cc.LastCalculatedWrongMana = Mathf.Max(0, intRef.wrongPigmentAmount);
        }
    }
}
