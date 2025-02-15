using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced
{
    /// <summary>
    /// An advanced version of EnemySO with more options.
    /// </summary>
    public class AdvancedEnemySO : EnemySO
    {
        /// <summary>
        /// This enemy's hidden effects.
        /// </summary>
        public List<HiddenEffectSO> hiddenEffects = [];
    }
}
