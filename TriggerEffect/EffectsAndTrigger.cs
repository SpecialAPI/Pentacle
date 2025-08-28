using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect
{

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
}
