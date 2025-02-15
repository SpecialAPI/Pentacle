using Pentacle.Advanced.Interfaces;
using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced.Patches.Ability
{
    [HarmonyPatch]
    internal static class MultiDisplayedStoredValuePatch_Ability
    {
        private static readonly MethodInfo desv_d = AccessTools.Method(typeof(MultiDisplayedStoredValuePatch_Ability), nameof(DisplayExtraStoredValues_Display));

        [HarmonyPatch(typeof(CombatVisualizationController), nameof(CombatVisualizationController.ShowcaseInfoAttackTooltip))]
        [HarmonyILManipulator]
        private static void DisplayExtraStoredValues_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<AbilitySO>(nameof(AbilitySO.GetAbilityLocData))))
                return;

            crs.Emit(OpCodes.Ldloc_1);
            crs.Emit(OpCodes.Ldloca, 2);
            crs.Emit(OpCodes.Call, desv_d);
        }

        private static AbilitySO DisplayExtraStoredValues_Display(AbilitySO ab, IReadOnlyDictionary<string, UnitStoreDataHolder> storedValues, ref string storedValueString)
        {
            if(ab == null || ab is not IMultipleStoredValueHolder holder)
                return ab;

            var displayedSV = holder.GetDisplayedStoredValues();

            if (storedValues == null || displayedSV == null)
                return ab;

            var sb = new StringBuilder();

            if(!string.IsNullOrEmpty(storedValueString))
                sb.AppendLine(storedValueString);

            foreach(var sv in displayedSV)
            {
                if (!storedValues.TryGetValue(sv._UnitStoreDataID, out var svHolder))
                    continue;

                if (!svHolder.TryGetUnitStoreDataToolTip(out var tooltip))
                    continue;

                sb.AppendLine(tooltip);
            }

            storedValueString = sb.ToString();
            return ab;
        }
    }
}
