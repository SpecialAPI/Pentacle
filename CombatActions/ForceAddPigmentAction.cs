using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Misc
{
    /// <summary>
    /// Produces a given amount of the given pigment, even if the pigment can't normally be produced.
    /// </summary>
    /// <param name="pigment">The pigment color to produce.</param>
    /// <param name="amount">How much pigment should be produced?</param>
    /// <param name="isGeneratorCharacter">Is the unit visually producing the pigment a character?</param>
    /// <param name="id">The id of the unit visually producing the pigment.</param>
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
