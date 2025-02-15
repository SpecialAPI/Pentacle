using FMODUnity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    public static class CombatUIExtensions
    {
        public static IEnumerator ModifyOverflowPigment(this CombatVisualizationController control, List<int> slots, List<ManaColorSO> pigment)
        {
            yield return control._manaOverflow.ModifyOverflowPigmentCoroutine(slots, pigment, control._mainManaBar._manaRandomizedEvent);
        }

        public static IEnumerator ModifyOverflowPigmentCoroutine(this ManaOverflowLayout layout, List<int> slots, List<ManaColorSO> pigment, string soundEvent)
        {
            RuntimeManager.PlayOneShot(soundEvent);

            for (int i = 0; i < Mathf.Min(slots.Count, pigment.Count); i++)
            {
                var s = slots[i];

                if (s >= layout.StoredSlots.Count)
                    continue;

                layout.StoredSlots[s] = pigment[i];
            }

            layout.UpdateTitle();
            layout.UpdateExposedSlots();
            layout.UpdateHiddenSlotText();
            layout.UpdateDamageText();

            yield return layout._waitForOverflowTimer;
        }
    }
}
