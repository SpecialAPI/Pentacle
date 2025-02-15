using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

namespace Pentacle.UnitExtension
{
    [HarmonyPatch]
    internal class HealthColorOptionsPatch
    {
        private static readonly Sprite ManaToggle_Disabled = AdvancedResourceLoader.LoadSprite("UI_ManaToggle_Disabled");

        [HarmonyPatch(typeof(CombatVisualizationController), nameof(CombatVisualizationController.FirstInitialization))]
        [HarmonyPrefix]
        private static void CopyButton(CombatVisualizationController __instance)
        {
            // Find lucky blue switch button
            var button = __instance.transform.Find("Canvas").Find("LowerZone").Find("InformationCanvas").Find("CharacterCostZone").Find("AttackCostLayout").Find("LuckyManaLayout").Find("ChangeLuckyBlueButton");

            // Find health layout
            var healthlayout = __instance.transform.Find("Canvas").Find("LowerZone").Find("InformationCanvas").Find("InfoZoneLayout").Find("PortraitZone").Find("PortraitHolder").Find("PortraitSprite").Find("CombatHealthBarLayout");

            // Make new button
            var newButton = Object.Instantiate(button.gameObject, healthlayout);
            newButton.SetActive(false);

            // For some reason normally the new button's position constantly gets changed so its no longer visible, this is a janky workaround but it works
            newButton.transform.localPosition = newButton.AddComponent<LocalPositionConstantSetter>().targetPos = new Vector3(125f, 0f, 0f);
            newButton.transform.localScale = new Vector3(3f, 3f, 3f);

            var buttonComp = newButton.GetComponent<Button>();
            var holder = healthlayout.gameObject.AddComponent<ChangeHealthColorButtonHolder>();

            // Cache the original button sprite to switch to it when the button is enabled
            holder.originalSprite = newButton.GetComponent<Image>().sprite;
            holder.button = newButton;

            // Create event to switch the active unit's health color
            buttonComp.onClick = new();
            buttonComp.onClick.AddListener(holder.ButtonClicked);

            // Override the disabled color
            var colors = buttonComp.colors;
            colors.disabledColor = __instance._characterCost._performAttackButton.colors.disabledColor;
            buttonComp.colors = colors;
        }

        [HarmonyPatch(typeof(CombatHealthBarLayout), nameof(CombatHealthBarLayout.SetInformation))]
        [HarmonyPrefix]
        private static void UpdateButton(CombatHealthBarLayout __instance)
        {
            if (CombatManager.Instance == null || CombatManager.Instance._combatUI == null)
                return;

            var buttonhold = __instance.GetComponent<ChangeHealthColorButtonHolder>();

            if (buttonhold == null || buttonhold.button == null)
                return;

            if (!CombatManager.Instance._stats.TryGetUnitOnField(CombatManager.Instance._combatUI.UnitInInfoID, CombatManager.Instance._combatUI.IsInfoFromCharacter, out var u))
                return;

            // Only enable the button if the unit has any additionl health colors
            var buttonActive = u != null && u.Ext().HealthColors.Count > 1;

            buttonhold.button.SetActive(buttonActive);
            buttonhold.button.transform.localPosition = new Vector3(125f, 0f, 0f);
        }

        [HarmonyPatch(typeof(AttackCostLayout), nameof(AttackCostLayout.UpdatePerformAttackButton))]
        [HarmonyPostfix]
        private static void UpdateButtonInteractability(AttackCostLayout __instance)
        {
            if (CombatManager.Instance == null || CombatManager.Instance._combatUI == null)
                return;

            var buttonhold = CombatManager.Instance._combatUI._infoZone._unitHealthBar.GetComponent<ChangeHealthColorButtonHolder>();

            if (buttonhold == null || buttonhold.button == null)
                return;

            var btn = buttonhold.button.GetComponent<Button>();
            var img = buttonhold.button.GetComponent<Image>();

            // Make the button interactable only if the player isn't input locked
            btn.interactable = !__instance.PlayerInputLocked;

            // Update the button's sprite
            img.sprite = btn.interactable ? buttonhold.originalSprite : ManaToggle_Disabled;
        }

        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.HealthColor), MethodType.Setter)]
        [HarmonyPatch(typeof(CharacterCombat), nameof(CharacterCombat.HealthColor), MethodType.Setter)]
        [HarmonyPostfix]
        private static void ChangeCharacterColorOption(IUnit __instance, ManaColorSO value)
        {
            var ext = __instance.Ext();

            // If a unit's health color gets updated, also update its current health option to that color
            ext.HealthColors[ext.HealthColorIndex % ext.HealthColors.Count] = value;
        }

        private class LocalPositionConstantSetter : MonoBehaviour
        {
            public Vector3 targetPos;

            public void LateUpdate()
            {
                transform.localPosition = targetPos;
            }
        }

        private class ChangeHealthColorButtonHolder : MonoBehaviour
        {
            public GameObject button;
            public Sprite originalSprite;

            public void ButtonClicked()
            {
                var manager = CombatManager.Instance;

                if (manager == null || manager._combatUI == null || manager._stats == null)
                    return;

                var ui = manager._combatUI;
                var stats = manager._stats;

                // Get the currently selected unit
                if (!stats.TryGetUnitOnField(ui.UnitInInfoID, ui.IsInfoFromCharacter, out var u))
                    return;

                var ext = u.Ext();
                var origIdx = ext.HealthColorIndex;

                // Switch to the next health color
                ext.HealthColorIndex = (ext.HealthColorIndex + 1) % ext.HealthColors.Count;

                if (u.ChangeHealthColor(ext.HealthColors[ext.HealthColorIndex]))
                    return;

                // If the health color change wasn't successful, revert to the original health color index
                ext.HealthColorIndex = origIdx;
            }
        }
    }
}
