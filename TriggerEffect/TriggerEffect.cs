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
}
