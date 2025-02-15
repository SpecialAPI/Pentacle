using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.Advanced
{
    /// <summary>
    /// An advanced version of CharacterSO with more options.
    /// </summary>
    public class AdvancedCharacterSO : CharacterSO
    {
        /// <summary>
        /// This character's hidden effects.
        /// </summary>
        public List<HiddenEffectSO> hiddenEffects = [];
    }
}
