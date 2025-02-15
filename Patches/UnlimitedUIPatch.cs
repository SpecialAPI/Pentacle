using Pentacle.Tools;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Yarn.Unity;

namespace Pentacle.Patches
{
    [HarmonyPatch]
    internal static class UnlimtedUIPatch
    {
        private static readonly MethodInfo udo_ab = AccessTools.Method(typeof(UnlimtedUIPatch), nameof(UnlimitedDialogueOptions_AddButtons));
        private static readonly MethodInfo udo_sss = AccessTools.Method(typeof(UnlimtedUIPatch), nameof(UnlimitedDialogueOptions_SetupSetState));
        private static readonly MethodInfo udo_cs = AccessTools.Method(typeof(UnlimtedUIPatch), nameof(UnlimitedDialogueOptions_CleanupSets));

        [HarmonyPatch(typeof(CombatVisualizationController), nameof(CombatVisualizationController.TrySetUpCostInformation))]
        [HarmonyPrefix]
        public static void UnlimitedCostSlots_AttackButton_Prefix(CombatVisualizationController __instance, ManaColorSO[] slotCost)
        {
            if (__instance._characterCost == null || __instance._characterCost._costSlots == null || slotCost == null)
                return;

            if (slotCost.Length <= __instance._costBarInfo.Length)
                return;

            var toAdd = slotCost.Length - __instance._costBarInfo.Length;
            var newCostSlots = __instance._characterCost._costSlots.ToList();

            for (var i = 0; i < toAdd; i++)
            {
                var stuff = Object.Instantiate(newCostSlots[0].gameObject, newCostSlots[0].transform.parent).GetComponent<ManaSlotLayout>();

                stuff.SetManaSlotIDs(ManaBarType.Cost, newCostSlots.Count);
                stuff.transform.SetAsFirstSibling(); // Attack button cost slots are in opposite order

                newCostSlots.Add(stuff);
            }

            __instance._costBarInfo = [.. __instance._costBarInfo, .. new CostSlotUIInfo[toAdd].Select(x => new CostSlotUIInfo())];
            __instance._characterCost._costSlots = [.. newCostSlots];
            __instance._characterCost.CurrentCost = [.. __instance._characterCost.CurrentCost, .. new ManaColorSO[toAdd].Populate(null)];
        }

        [HarmonyPatch(typeof(AttackCostLayout), nameof(AttackCostLayout.SetSlotActivity))]
        [HarmonyPostfix]
        public static void UnlimitedCostSlots_AttackButton_DeactivateNewSlots_Postfix(AttackCostLayout __instance, int index, bool enabled)
        {
            if (index < 6)
                return;

            // Hiding attack button slots doesn't deactivate their gameObjects, deactivate them manually so that it doesn't cause weird scaling for abilities not affected by unlimited costs
            __instance._costSlots[index].gameObject.SetActive(enabled);
        }

        [HarmonyPatch(typeof(AttackSlotLayout), nameof(AttackSlotLayout.FillCostInfo))]
        [HarmonyPrefix]
        public static void UnlimitedCostSlots_AbilitySlot_Prefix(AttackSlotLayout __instance, ManaColorSO[] costs)
        {
            if (__instance._PigmentCosts == null || costs == null)
                return;

            if (__instance._PigmentCosts.Length >= costs.Length)
                return;

            var toAdd = costs.Length - __instance._PigmentCosts.Length;
            var newCostSlots = __instance._PigmentCosts.ToList();

            for(var i = 0; i < toAdd; i++)
            {
                var stuff = Object.Instantiate(newCostSlots[0].gameObject, newCostSlots[0].transform.parent).GetComponent<Image>();

                newCostSlots.Add(stuff);
            }

            __instance._PigmentCosts = [.. newCostSlots];
        }

        [HarmonyPatch(typeof(Info_AttackLayout), nameof(Info_AttackLayout.SetInformation))]
        [HarmonyPrefix]
        public static void UnlimitedCostSlots_InfoUI_Prefix(Info_AttackLayout __instance, ManaColorSO[] cost)
        {
            if (__instance._costHolders == null || cost == null)
                return;

            if (__instance._costHolders.Length >= cost.Length)
                return;

            var toAdd = cost.Length - __instance._costHolders.Length;

            var newCostSlots = __instance._costHolders.ToList();
            var newCostImages = __instance._costImages.ToList();

            for (var i = 0; i < toAdd; i++)
            {
                var stuff = Object.Instantiate(newCostSlots[0].gameObject, newCostSlots[0].transform.parent);

                stuff.transform.SetAsFirstSibling(); // Attack button cost slots are in opposite order

                // Add both the new cost object as well as the new cost image to the arrays
                newCostSlots.Add(stuff);
                newCostImages.Add(stuff.GetComponentInChildren<Image>());
            }

            __instance._costHolders = [.. newCostSlots];
            __instance._costImages = [.. newCostImages];
        }

        [HarmonyPatch(typeof(AttackListLayout), nameof(AttackListLayout.SetCharacterInformation))]
        [HarmonyPrefix]
        public static void UnlimitedAbilities_Characters_Combat_Prefix(AttackListLayout __instance, List<AbilitySO> attacks)
        {
            if (attacks == null || __instance._attackSlots == null)
                return;

            if (__instance._attackSlots.Length >= attacks.Count || __instance._attackSlots.Length <= 0)
                return;

            var toAdd = attacks.Count - __instance._attackSlots.Length;
            var newSlots = __instance._attackSlots.ToList();

            for (var i = 0; i < toAdd; i++)
            {
                var stuff = Object.Instantiate(newSlots[0].gameObject, newSlots[0].transform.parent).GetComponent<AttackSlotLayout>();

                stuff.AttackSlotID = newSlots.Count; // Set attack slot id so that clicking selects the correct ability
                newSlots.Add(stuff);
            }

            __instance._attackSlots = [.. newSlots];
        }

        [HarmonyPatch(typeof(PartyCharacterInformationUIHandler), nameof(PartyCharacterInformationUIHandler.SetAbilityInformation))]
        [HarmonyPrefix]
        public static void UnlimitedAbilities_Characters_Party_Prefix(PartyCharacterInformationUIHandler __instance, List<CharacterAbility> abilities)
        {
            if (abilities == null || __instance._abilityUILayouts == null)
                return;

            if (__instance._abilityUILayouts.Length >= abilities.Count || __instance._abilityUILayouts.Length <= 0)
                return;

            var toAdd = abilities.Count - __instance._abilityUILayouts.Length;
            var newSlots = __instance._abilityUILayouts.ToList();

            for(var i = 0; i < toAdd; i++)
            {
                var stuff = Object.Instantiate(newSlots[0].gameObject, newSlots[0].transform.parent).GetComponent<AbilityUILayout>();

                newSlots.Add(stuff);
            }

            __instance._abilityUILayouts = [.. newSlots];
        }

        [HarmonyPatch(typeof(SelectableCharacterInformationLayout), nameof(SelectableCharacterInformationLayout.SetAbilities))]
        [HarmonyPrefix]
        public static void UnlimitedAbilities_Characters_MainMenu_Prefix(SelectableCharacterInformationLayout __instance, CharacterSO character, int rank)
        {
            if (__instance == null || character == null || !character.HasRankedData || character.rankedData.Count <= rank)
                return;

            var dat = character.rankedData[rank];

            if (dat == null || dat.rankAbilities == null)
                return;

            if (__instance._abilityUILayouts.Length >= dat.rankAbilities.Length)
                return;

            var toAdd = dat.rankAbilities.Length - __instance._abilityUILayouts.Length;
            var newSlots = __instance._abilityUILayouts.ToList();

            for(var i = 0; i < toAdd; i++)
            {
                var stuff = Object.Instantiate(newSlots[0].gameObject, newSlots[0].transform.parent).GetComponent<AbilityUIMainMenuLayout>();
                var trigger = stuff.GetComponent<EventTrigger>();

                var idx = newSlots.Count;

                newSlots.Add(stuff);

                // In order to make hovering over the icon display the correct ability we need to change the event trigger
                if (trigger == null)
                    continue;

                // Find the trigger for hovering over
                var tr = trigger.triggers.FirstOrDefault(x => x.eventID == EventTriggerType.PointerEnter);

                if (tr == null)
                    continue;

                // Override its callback
                tr.callback = new();
                tr.callback.AddListener(x => __instance.OnAbilityEnter(idx));
            }

            __instance._abilityUILayouts = [.. newSlots];
        }

        [HarmonyPatch(typeof(DialogueUI), nameof(DialogueUI.DoRunOptions), MethodType.Enumerator)]
        [HarmonyILManipulator]
        public static void UnlimitedDialogueOptions_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<Yarn.OptionSet>($"get_{nameof(Yarn.OptionSet.Options)}")))
                return;

            crs.Emit(OpCodes.Ldloc_1);
            crs.Emit(OpCodes.Call, udo_ab);

            if (!crs.JumpBeforeNext(x => x.MatchLdfld<DialogueUI>(nameof(DialogueUI.onOptionsStart))))
                return;

            crs.Emit(OpCodes.Ldloc_3);
            crs.Emit(OpCodes.Call, udo_sss);

            if (!crs.JumpBeforeNext(x => x.MatchLdfld<DialogueUI>(nameof(DialogueUI.onOptionsEnd))))
                return;

            crs.Emit(OpCodes.Call, udo_cs);
        }

        public static Yarn.OptionSet.Option[] UnlimitedDialogueOptions_AddButtons(Yarn.OptionSet.Option[] curr, DialogueUI dialogue)
        {
            if(dialogue == null || dialogue.optionButtons == null || dialogue.optionButtons.Count <= 1)
                return curr;

            var buttons = dialogue.optionButtons;
            var setsHandler = dialogue.GetComponent<DialogueOptionSetsHandler>();

            if(setsHandler == null)
            {
                setsHandler = dialogue.gameObject.AddComponent<DialogueOptionSetsHandler>();

                setsHandler.optionsInSet = buttons.Count - 1;
                setsHandler.dialogue = dialogue;

                var target = buttons[0];

                var btn = setsHandler.nextSetButton = Object.Instantiate(target.gameObject, target.transform.parent).GetComponent<Button>();
                btn.transform.localPosition = dialogue.optionButtons[setsHandler.optionsInSet].transform.localPosition;

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(setsHandler.NextSet);
                btn.gameObject.SetActive(false);

                var nextSetText = "->";

                var unityText = btn.GetComponentInChildren<Text>();
                if (unityText != null)
                    unityText.text = nextSetText;

                var tmpText = btn.GetComponentInChildren<TMP_Text>();
                if (tmpText != null)
                    tmpText.text = nextSetText;
            }

            var buttonsInSet = setsHandler.optionsInSet;

            if (curr == null || dialogue.optionButtons.Count >= curr.Length)
                return curr;

            var toAdd = curr.Length - buttons.Count;

            for(var i = 0; i < toAdd; i++)
            {
                var newIdx = buttons.Count;

                var target = buttons[newIdx % buttonsInSet];
                var stuff = Object.Instantiate(target.gameObject, target.transform.parent).GetComponent<Button>();

                buttons.Add(stuff);
            }

            return curr;
        }

        public static DialogueUI UnlimitedDialogueOptions_SetupSetState(DialogueUI dialogue, Yarn.OptionSet.Option[] options)
        {
            if (dialogue == null)
                return dialogue;

            var setsHandler = dialogue.GetComponent<DialogueOptionSetsHandler>();

            if (setsHandler == null)
                return dialogue; // uh oh

            setsHandler.SetupSetsFromOptions(options);
            return dialogue;
        }

        public static DialogueUI UnlimitedDialogueOptions_CleanupSets(DialogueUI dialogue)
        {
            if (dialogue == null)
                return dialogue;

            var setsHandler = dialogue.GetComponent<DialogueOptionSetsHandler>();

            if (setsHandler == null)
                return dialogue;

            setsHandler.CleanupSets();
            return dialogue;
        }

        private class DialogueOptionSetsHandler : MonoBehaviour
        {
            public int optionsInSet;
            public DialogueUI dialogue;

            public Button nextSetButton;

            private int currentSet = 0;
            private int sets = 1;

            public void SetupSetsFromOptions(Yarn.OptionSet.Option[] opt)
            {
                sets = 1;
                currentSet = 0;

                nextSetButton.gameObject.SetActive(false);
                dialogue.optionButtons[optionsInSet].transform.localPosition = nextSetButton.transform.localPosition;

                if (opt.Length <= (optionsInSet + 1))
                    return;

                sets = Mathf.CeilToInt(opt.Length / (float)optionsInSet);

                for(var i = optionsInSet; i < dialogue.optionButtons.Count; i++)
                    dialogue.optionButtons[i].gameObject.SetActive(false);

                nextSetButton.gameObject.SetActive(true);
            }

            public void CleanupSets()
            {
                currentSet = 0;
                sets = 0;

                nextSetButton.gameObject.SetActive(false);
                dialogue.optionButtons[optionsInSet].transform.localPosition = nextSetButton.transform.localPosition;
            }

            public void NextSet()
            {
                for(var i = 0; i < dialogue.optionButtons.Count; i++)
                {
                    var btn = dialogue.optionButtons[i];

                    if (i == optionsInSet)
                        btn.transform.localPosition = nextSetButton.transform.localPosition;

                    btn.gameObject.SetActive(false);
                }

                currentSet = (currentSet + 1) % sets;

                var startIdx = currentSet * optionsInSet;
                var end = startIdx + optionsInSet;

                if (sets <= 1 || end >= dialogue.optionButtons.Count)
                    end = dialogue.optionButtons.Count;

                for(var i = startIdx; i < end; i++)
                {
                    var btn = dialogue.optionButtons[i];

                    if (i == optionsInSet && sets > 1)
                        btn.transform.localPosition = dialogue.optionButtons[0].transform.localPosition;

                    btn.gameObject.SetActive(true);
                }
            }
        }
    }
}
