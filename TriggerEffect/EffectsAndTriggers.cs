using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect
{

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
