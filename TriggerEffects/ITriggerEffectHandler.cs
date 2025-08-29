using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffects
{
    /// <summary>
    /// An interface for storing information about trigger effect handlers.
    /// </summary>
    public interface ITriggerEffectHandler
    {
        /// <summary>
        /// The trigger effect handler's display name.
        /// </summary>
        string DisplayedName { get; }
        /// <summary>
        /// The trigger effect handler's sprite.
        /// </summary>
        Sprite Sprite { get; }

        /// <summary>
        /// Tries to get a combat action that shows the trigger effect handler's activation.
        /// </summary>
        /// <param name="unitId">Determines the ID of the unit the action should play on.</param>
        /// <param name="isUnitCharacter">Determines if the action should play on a character or an enemy.</param>
        /// <param name="consumed">Determines if the action should say that the trigger effect handler was consumed or not. This usually only applies to items.</param>
        /// <param name="action">Outputs the combat action that shows the trigger effect handler's activation.</param>
        /// <returns>True if getting the combat action was successful, false otherwise.</returns>
        bool TryGetPopupUIAction(int unitId, bool isUnitCharacter, bool consumed, out CombatAction action);
    }
}
