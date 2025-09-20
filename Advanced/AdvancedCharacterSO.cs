using Pentacle.HiddenPassiveEffects;
using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced
{
    /// <summary>
    /// An extended version of the CharacterSO class.
    /// </summary>
    public class AdvancedCharacterSO : CharacterSO
    {
        /// <summary>
        /// A list of this character's hidden passive effects.
        /// </summary>
        public List<HiddenEffectSO> hiddenEffects = [];
    }
}
