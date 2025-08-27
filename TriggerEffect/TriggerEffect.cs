using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect
{
    /// <summary>
    /// The base class for effects that can do something when a certain event happens.
    /// <para>Trigger effects are not to be confused with regular effects. Trigger effects are primarly designed to generalize the effects of passives and items. Trigger effects can perform regular effects (see <see cref="BasicTriggerEffects.PerformEffectTriggerEffect"/>)</para>
    /// </summary>
    public abstract class TriggerEffect
    {
        /// <summary>
        /// Performs this trigger effect.
        /// </summary>
        /// <param name="sender">The unit that triggered this trigger effect.</param>
        /// <param name="args">Addiional trigger-sent argument. This will always be null for connection- and disconnection-triggered trigger effects.</param>
        /// <param name="triggerInfo">The triggered effect that stores activation information about this trigger effect.</param>
        /// <param name="extraInfo">Extra information about how this trigger effect was triggered.</param>
        public abstract void DoEffect(IUnit sender, object args, TriggeredEffect triggerInfo, TriggerEffectExtraInfo extraInfo);

        /// <summary>
        /// If true, the trigger effect handler will not show the trigger effect's activation automatically (for example, an item using this trigger effect will not do the "item used" popup). 
        /// <para>This can be used for trigger effects that either delay the activation display or only trigger it under certain conditions. The trigger effect can manually show the acticvation by using <see cref="TriggerEffectExtraInfo.TryGetPopupUIAction"/>. Note that this doesn't automatically check for <see cref="TriggeredEffect.doesPopup"/>.</para>
        /// </summary>
        public virtual bool ManuallyHandlePopup => false;
    }

    /// <summary>
    /// A class that provides additional information about how a trigger effect is triggered.
    /// </summary>
    public class TriggerEffectExtraInfo
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

    /// <summary>
    /// A class that stores information about how a trigger effect should be triggered upon a certain event happening.
    /// </summary>
    public class TriggeredEffect
    {
        /// <summary>
        /// The trigger effect that should be triggered. If this is null, only the visual activation (as well as consumption for items) will happen.
        /// </summary>
        public TriggerEffect effect;
        /// <summary>
        /// The effector conditions for the trigger effect. All of these conditions need to be fulfilled for the trigger effect to be triggered successfully.
        /// </summary>
        public List<EffectorConditionSO> conditions;
        /// <summary>
        /// Determines if the trigger effect should be performed immediately or as a subaction.
        /// </summary>
        public bool immediate;
        /// <summary>
        /// Determines if a trigger effect handler using the trigger effect should visually show its activation.
        /// </summary>
        public bool doesPopup = true;
        /// <summary>
        /// Determines if an item using this trigger effect should be consumed after triggering it.
        /// </summary>
        public bool getsConsumed;
    }

    /// <summary>
    /// A class that stores information about how a trigger effect should be triggered upon a trigger call being posted.
    /// </summary>
    public class EffectsAndTrigger : TriggeredEffect
    {
        /// <summary>
        /// The trigger call that should trigger the trigger effect.
        /// </summary>
        public string trigger;

        /// <summary>
        /// Returns an enumerable containing the names of all trigger calls that trigger the trigger effect.
        /// </summary>
        /// <returns>An enumerable of all trigger calls that trigger the trigger effect.</returns>
        public virtual IEnumerable<string> TriggerStrings()
        {
            if (!string.IsNullOrEmpty(trigger))
                yield return trigger;
        }
    }

    /// <summary>
    /// Information about an effect that should be performed on any of the given triggers.
    /// </summary>
    public class EffectsAndTriggers : EffectsAndTrigger
    {
        /// <summary>
        /// A list of trigger calls that should trigger the trigger effect.
        /// </summary>
        public List<string> triggers = [];

        /// <inheritdoc/>
        public override IEnumerable<string> TriggerStrings()
        {
            foreach (var b in base.TriggerStrings())
                yield return b;

            foreach (var t in triggers)
            {
                if (!string.IsNullOrEmpty(t))
                    yield return t;
            }
        }
    }
}
