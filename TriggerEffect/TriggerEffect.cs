using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect
{
    /// <summary>
    /// The base class for effects that do something on a trigger.
    /// </summary>
    public abstract class TriggerEffect
    {
        /// <summary>
        /// Triggers this Trigger Effect.
        /// </summary>
        /// <param name="sender">The unit that triggered this effect.</param>
        /// <param name="args">Addiional arguments for the trigger. Can be null.</param>
        /// <param name="triggerInfo">Information about this effect.</param>
        /// <param name="activator">This effect's activator, for example an item or passive that this effect belongs to.</param>
        public abstract void DoEffect(IUnit sender, object args, TriggeredEffect triggerInfo, object activator = null);
    }

    /// <summary>
    /// Information about an effect and how it should be performed.
    /// </summary>
    public class TriggeredEffect
    {
        /// <summary>
        /// The effect that should be performed.
        /// </summary>
        public TriggerEffect effect;
        /// <summary>
        /// Effector conditions for this effect to be performed.
        /// </summary>
        public List<EffectorConditionSO> conditions;
        /// <summary>
        /// Should the effect be performed immediately when the trigger happens?
        /// </summary>
        public bool immediate;
        /// <summary>
        /// Should the effect trigger the passive/item popup? Does nothing for Hidden Effects.
        /// </summary>
        public bool doesPopup = true;
        /// <summary>
        /// Should the item be consumed when this effect happens? Does nothing for passives or Hidden Effects.
        /// </summary>
        public bool getsConsumed;
    }

    /// <summary>
    /// Information about an effect that should be performed on a trigger.
    /// </summary>
    public class EffectsAndTrigger : TriggeredEffect
    {
        /// <summary>
        /// The trigger for this effect.
        /// </summary>
        public string trigger;

        /// <summary>
        /// Gets all triggers that should trigger this effect.
        /// </summary>
        /// <returns>An enumerable of all triggers for this effect.</returns>
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
        /// A list of triggers for this effect.
        /// </summary>
        public List<string> triggers = [];

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
