using FMODUnity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Tools
{
    /// <summary>
    /// Static class that provides extension methods for CombatVisualizationController and other classes related to combat UI.
    /// </summary>
    public static class CombatUIExtensions
    {
        /// <summary>
        /// Modifies the visually displayed pigment in the overflow.
        /// </summary>
        /// <param name="control">The combat UI.</param>
        /// <param name="slots">The indexes of the pigments that should be changed. This list's length should match the length of <paramref name="slots"/>.</param>
        /// <param name="pigment">The new pigment colors for the pigments in those slots. This list's length should match the length of <paramref name="pigment"/>.</param>
        /// <returns></returns>
        public static IEnumerator ModifyOverflowPigment(this CombatVisualizationController control, List<int> slots, List<ManaColorSO> pigment)
        {
            yield return control._manaOverflow.ModifyOverflowPigmentCoroutine(slots, pigment, control._mainManaBar._manaRandomizedEvent);
        }

        /// <summary>
        /// Modifies the visually displayed pigment in the overflow.
        /// </summary>
        /// <param name="layout">The combat UI's overflow layout.</param>
        /// <param name="slots">The indexes of the pigments that should be changed. This list's length should match the length of <paramref name="slots"/>.</param>
        /// <param name="pigment">The new pigment colors for the pigments in those slots. This list's length should match the length of <paramref name="pigment"/>.</param>
        /// <param name="soundEvent">The sound that should be played when modifying the pigment.</param>
        /// <returns></returns>
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
