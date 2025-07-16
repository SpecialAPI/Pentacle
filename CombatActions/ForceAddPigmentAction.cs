using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Misc
{
    /// <summary>
    /// An immediate action that produces a certain amount of the given pigment color, even if pigment of that color can't normally be produced.
    /// </summary>
    /// <param name="pigment">The pigment color that should be produced, as a ManaColorSO object.</param>
    /// <param name="amount">The amount of pigment that should be produced.</param>
    /// <param name="isGeneratorCharacter">Determines whether the pigment will be visually produced by a character or an enemy.</param>
    /// <param name="id">The id of the unit that will be visually producing the pigment.</param>
    public class ForceAddPigmentAction(ManaColorSO pigment, int amount, bool isGeneratorCharacter, int id) : IImmediateAction
    {
        public void Execute(CombatStats stats)
        {
            if (pigment == null)
                return;

            stats.MainManaBar.AddManaAmount(pigment, amount, stats.GenerateUnitJumpInformation(id, isGeneratorCharacter));
        }
    }
}
