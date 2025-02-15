using Pentacle.CustomEvent.Args;
using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Patches
{
    [HarmonyPatch]
    internal static class CustomForbiddenFruit
    {
        private static MethodInfo tff_e_t = AccessTools.Method(typeof(CustomForbiddenFruit), nameof(TriggerForbiddenFruit_Enemies_Trigger));
        private static MethodInfo tff_c_t = AccessTools.Method(typeof(CustomForbiddenFruit), nameof(TriggerForbiddenFruit_Characters_Trigger));

        [HarmonyPatch(typeof(TimelineEndReachedAction), nameof(TimelineEndReachedAction.Execute), MethodType.Enumerator)]
        [HarmonyILManipulator]
        private static void TriggerForbiddenFruit_Enemies_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.TryGotoNext(x => x.MatchCallOrCallvirt<CombatStats>(nameof(CombatStats.TryTriggerEnemyForbiddenFruit))))
                return;

            crs.Emit(OpCodes.Call, tff_e_t);
        }

        private static void TriggerForbiddenFruit_Enemies_Trigger()
        {
            TryTriggerCustomForbiddenFruit(false);
        }

        [HarmonyPatch(typeof(PlayerTurnEndAction), nameof(PlayerTurnEndAction.Execute), MethodType.Enumerator)]
        [HarmonyILManipulator]
        private static void TriggerForbiddenFruit_Characters_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.TryGotoNext(x => x.MatchCallOrCallvirt<SlotsCombat>(nameof(SlotsCombat.CharacterSlotTurnEnd))))
                return;

            crs.Emit(OpCodes.Call, tff_c_t);
        }

        private static void TriggerForbiddenFruit_Characters_Trigger()
        {
            TryTriggerCustomForbiddenFruit(true);
        }

        private static void TryTriggerCustomForbiddenFruit(bool characters)
        {
            var stats = CombatManager.Instance._stats;

            var firstUnits = new List<IUnit>();
            var totalUnits = stats.UnitsOnField();

            if (characters)
                firstUnits.AddRange(stats.CharactersOnField.Values);
            else
                firstUnits.AddRange(stats.EnemiesOnField.Values);

            var matches = new List<(int matchIndex, string ffId, string passiveName, Sprite passiveIcon)>();

            var canMatchBoolRef = new BooleanReference(false);
            var isFirstUnitBoolRef = new BooleanReference(false);
            var ffIdStringRef = new StringReference("");
            var passiveNameStringRef = new StringReference("");
            var matchInfo = new ForbiddenFruitCanMatchInfo(canMatchBoolRef, isFirstUnitBoolRef, ffIdStringRef, null, passiveNameStringRef, null);

            while(firstUnits.Count > 0)
            {
                var idx = Random.Range(0, firstUnits.Count);

                var firstUnit = firstUnits[idx];
                firstUnits.RemoveAt(idx);
                totalUnits.Remove(firstUnit);

                if (firstUnit == null)
                    continue;

                ffIdStringRef.value = string.Empty;
                CombatManager.Instance.PostNotification(CustomTriggers.GetForbiddenFruitID, firstUnit, ffIdStringRef);
                var firstUnitId = ffIdStringRef.value;

                matches.Clear();
                for(var i = 0; i < totalUnits.Count; i++)
                {
                    var secondUnit = totalUnits[i];

                    if (secondUnit == null || firstUnit == secondUnit)
                        continue;

                    ffIdStringRef.value = string.Empty;
                    CombatManager.Instance.PostNotification(CustomTriggers.GetForbiddenFruitID, secondUnit, ffIdStringRef);
                    var secondUnitId = ffIdStringRef.value;

                    canMatchBoolRef.value = false;
                    isFirstUnitBoolRef.value = true;
                    ffIdStringRef.value = secondUnitId;
                    matchInfo.OtherUnit = secondUnit;
                    passiveNameStringRef.value = "";
                    matchInfo.PassiveSprite = null;
                    CombatManager.Instance.PostNotification(CustomTriggers.CanForbiddenFruitMatch, firstUnit, matchInfo);
                    var canMatch1 = canMatchBoolRef.value;

                    if (!canMatch1)
                        continue;

                    canMatchBoolRef.value = false;
                    isFirstUnitBoolRef.value = false;
                    ffIdStringRef.value = firstUnitId;
                    matchInfo.OtherUnit = firstUnit;
                    passiveNameStringRef.value = "";
                    matchInfo.PassiveSprite = null;
                    CombatManager.Instance.PostNotification(CustomTriggers.CanForbiddenFruitMatch, secondUnit, matchInfo);
                    var canMatch2 = canMatchBoolRef.value;
                    var secondUnitPassiveName = passiveNameStringRef.value;
                    var secondUnitPassiveSprite = matchInfo.PassiveSprite;

                    if (!canMatch2)
                        continue;

                    matches.Add((i, secondUnitId, secondUnitPassiveName, secondUnitPassiveSprite));
                }

                if (matches.Count <= 0)
                    continue;

                var randomMatchIdx = Random.Range(0, matches.Count);
                var match = matches[randomMatchIdx];
                var matchIndex = match.matchIndex;

                var matchUnit = totalUnits[matchIndex];
                totalUnits.RemoveAt(matchIndex);

                if (matchUnit == null)
                    continue;

                var successfulMatchInfo = new ForbiddenFruitSuccessfulMatchInfo(new(match.ffId), matchUnit, match.passiveName, match.passiveIcon);
                CombatManager.Instance.PostNotification(CustomTriggers.TriggerForbiddenFruit, firstUnit, successfulMatchInfo);
            }
        }
    }
}
