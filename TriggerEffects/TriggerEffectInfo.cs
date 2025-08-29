using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffects
{

    /// <summary>
    /// A class that stores information about how a trigger effect should be triggered upon a certain event happening.
    /// </summary>
    public class TriggerEffectInfo
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
}
