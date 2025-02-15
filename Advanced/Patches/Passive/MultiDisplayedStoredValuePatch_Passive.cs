using Pentacle.Advanced.Interfaces;
using Pentacle.Advanced.Patches.Ability;
using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced.Patches.Passive
{
    [HarmonyPatch]
    internal static class MultiDisplayedStoredValuePatch_Passive
    {
        private static readonly MethodInfo desv_d = AccessTools.Method(typeof(MultiDisplayedStoredValuePatch_Ability), nameof(DisplayExtraStoredValues_Display));

        [HarmonyPatch(typeof(CombatVisualizationController), nameof(CombatVisualizationController.ShowcaseInfoPassiveTooltip))]
        [HarmonyILManipulator]
        private static void DisplayExtraStoredValues_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<BasePassiveAbilitySO>(nameof(BasePassiveAbilitySO.GetPassiveLocData))))
                return;

            crs.Emit(OpCodes.Ldloc_1);
            crs.Emit(OpCodes.Ldloca, 2);
            crs.Emit(OpCodes.Call, desv_d);
        }

        private static BasePassiveAbilitySO DisplayExtraStoredValues_Display(BasePassiveAbilitySO pa, IReadOnlyDictionary<string, UnitStoreDataHolder> storedValues, ref string storedValueString)
        {
            if (pa == null || pa is not IMultipleStoredValueHolder holder)
                return pa;

            var displayedSV = holder.GetDisplayedStoredValues();

            if (storedValues == null || displayedSV == null)
                return pa;

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(storedValueString))
                sb.AppendLine(storedValueString);

            foreach (var sv in displayedSV)
            {
                if (!storedValues.TryGetValue(sv._UnitStoreDataID, out var svHolder))
                    continue;

                if (!svHolder.TryGetUnitStoreDataToolTip(out var tooltip))
                    continue;

                sb.AppendLine(tooltip);
            }

            storedValueString = sb.ToString();
            return pa;
        }
    }
}
