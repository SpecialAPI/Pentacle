using Pentacle.HiddenPassiveEffects;
using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced
{
    /// <summary>
    /// An extended version of the EnemySO class.
    /// </summary>
    public class AdvancedEnemySO : EnemySO
    {
        /// <summary>
        /// A list of this enemy's hidden passive effects.
        /// </summary>
        public List<HiddenPassiveEffectSO> hiddenEffects = [];
    }
}
