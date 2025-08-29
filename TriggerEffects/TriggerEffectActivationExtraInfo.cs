using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffects
{

    /// <summary>
    /// A class that provides additional information about how a trigger effect is triggered.
    /// </summary>
    public class TriggerEffectActivationExtraInfo
    {
        /// <summary>
        /// The trigger effect handler that triggered the trigger effect.
        /// </summary>
        public ITriggerEffectHandler handler;
        /// <summary>
        /// What event triggered the trigger effect.
        /// </summary>
        public TriggerEffectActivation activation;

        /// <summary>
        /// Tries to get a combat action that shows the trigger effect handler's activation.
        /// </summary>
        /// <param name="unitId">Determines the ID of the unit the action should play on.</param>
        /// <param name="isUnitCharacter">Determines if the action should play on a character or an enemy.</param>
        /// <param name="consumed">Determines if the action should say that the trigger effect handler was consumed or not. This usually only applies to items.</param>
        /// <param name="action">Outputs the combat action that shows the trigger effect handler's activation.</param>
        /// <returns>True if getting the combat action was successful, false otherwise.</returns>
        public bool TryGetPopupUIAction(int unitId, bool isUnitCharacter, bool consumed, out CombatAction action)
        {
            return handler.TryGetPopupUIAction(unitId, isUnitCharacter, consumed, out action);
        }
    }

    /// <summary>
    /// Types of events that can trigger a trigger effect.
    /// </summary>
    public enum TriggerEffectActivation
    {
        /// <summary>
        /// The trigger effect was triggered by a trigger call being posted.
        /// </summary>
        Trigger,
        /// <summary>
        /// The trigger effect was triggered by the trigger effect handler being connected.
        /// </summary>
        Connection,
        /// <summary>
        /// The trigger effect was triggered by the trigger effect handler being disconnected.
        /// </summary>
        Disconnection,
    }
}
